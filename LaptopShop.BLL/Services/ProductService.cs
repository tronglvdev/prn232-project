using LaptopShop.BLL.DTOs;
using LaptopShop.DAL.Entities;
using LaptopShop.DAL.Repositories;

namespace LaptopShop.BLL.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(string searchTerm = null, string[] factory = null, string[] target = null, string[] price = null, string sort = null)
    {
        var products = await _productRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                           (p.Factory != null && p.Factory.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }
        if (factory != null && factory.Length > 0)
        {
            products = products.Where(p => p.Factory != null && factory.Contains(p.Factory, StringComparer.OrdinalIgnoreCase));
        }
        if (target != null && target.Length > 0)
        {
            products = products.Where(p => p.Target != null && target.Contains(p.Target, StringComparer.OrdinalIgnoreCase));
        }
        if (price != null && price.Length > 0)
        {
            products = products.Where(p =>
                (price.Contains("duoi-10-trieu") && p.Price < 10000000) ||
                (price.Contains("10-15-trieu") && p.Price >= 10000000 && p.Price < 15000000) ||
                (price.Contains("15-20-trieu") && p.Price >= 15000000 && p.Price <= 20000000) ||
                (price.Contains("tren-20-trieu") && p.Price > 20000000)
            );
        }
        if (!string.IsNullOrEmpty(sort))
        {
            if (sort == "gia-tang-dan") products = products.OrderBy(p => p.Price);
            else if (sort == "gia-giam-dan") products = products.OrderByDescending(p => p.Price);
        }

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Image = p.Image,
            DetailDesc = p.DetailDesc,
            ShortDesc = p.ShortDesc,
            Quantity = p.Quantity,
            Sold = p.Sold,
            Factory = p.Factory,
            Target = p.Target
        });
    }

    public async Task<ProductDto> GetProductByIdAsync(long id)
    {
        var p = await _productRepository.GetByIdAsync(id);
        if (p == null) return null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Image = p.Image,
            DetailDesc = p.DetailDesc,
            ShortDesc = p.ShortDesc,
            Quantity = p.Quantity,
            Sold = p.Sold,
            Factory = p.Factory,
            Target = p.Target
        };
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Image = productDto.Image,
            DetailDesc = productDto.DetailDesc,
            ShortDesc = productDto.ShortDesc,
            Quantity = productDto.Quantity,
            Sold = productDto.Sold,
            Factory = productDto.Factory,
            Target = productDto.Target
        };

        await _productRepository.InsertAsync(product);
        await _productRepository.SaveAsync();

        productDto.Id = product.Id;
        return productDto;
    }

    public async Task UpdateProductAsync(long id, ProductDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product != null)
        {
            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Image = productDto.Image;
            product.DetailDesc = productDto.DetailDesc;
            product.ShortDesc = productDto.ShortDesc;
            product.Quantity = productDto.Quantity;
            product.Sold = productDto.Sold;
            product.Factory = productDto.Factory;
            product.Target = productDto.Target;

            _productRepository.Update(product);
            await _productRepository.SaveAsync();
        }
    }

    public async Task DeleteProductAsync(long id)
    {
        await _productRepository.DeleteAsync(id);
        await _productRepository.SaveAsync();
    }
}
