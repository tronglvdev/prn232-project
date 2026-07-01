using LaptopShop.BLL.DTOs;
using LaptopShop.BLL.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string[]? factory, [FromQuery] string[]? target, [FromQuery] string[]? price, [FromQuery] string? sort)
    {
        var products = await _productService.GetAllProductsAsync(name, factory, target, price, sort);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _productService.CreateProductAsync(productDto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _productService.UpdateProductAsync(id, productDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
