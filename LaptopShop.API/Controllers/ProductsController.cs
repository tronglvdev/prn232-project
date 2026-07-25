using LaptopShop.BLL.DTOs;
using LaptopShop.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;

namespace LaptopShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        // Trả về toàn bộ dữ liệu, OData ([EnableQuery]) sẽ tự động parse URL (chứa $filter, $orderby) và lọc dữ liệu giúp ta
        var products = await _productService.GetAllProductsAsync(null, null, null, null, null);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _productService.CreateProductAsync(productDto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _productService.UpdateProductAsync(id, productDto);
        return NoContent();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
