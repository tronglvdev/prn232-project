using LaptopShop.BLL.DTOs;

namespace LaptopShop.BLL.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(string searchTerm = null, string[] factory = null, string[] target = null, string[] price = null, string sort = null);
    Task<ProductDto> GetProductByIdAsync(long id);
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task UpdateProductAsync(long id, ProductDto productDto);
    Task DeleteProductAsync(long id);
}
