using System.Text.Json;
using LaptopShop.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LaptopShop.Web.Controllers;

public class StoreController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StoreController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index([FromQuery] string? name, [FromQuery] string[]? factory, [FromQuery] string[]? target, [FromQuery] string[]? price, [FromQuery] string? sort)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var query = new List<string>();
        
        if (!string.IsNullOrEmpty(name)) query.Add($"name={name}");
        if (factory != null && factory.Length > 0)
        {
            foreach(var f in factory) query.Add($"factory={f}");
        }
        if (target != null && target.Length > 0)
        {
            foreach(var t in target) query.Add($"target={t}");
        }
        if (price != null && price.Length > 0)
        {
            foreach(var p in price) query.Add($"price={p}");
        }
        if (!string.IsNullOrEmpty(sort)) query.Add($"sort={sort}");

        var url = "Products" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<ProductDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(products);
        }
        return View(new List<ProductDto>());
    }

    public async Task<IActionResult> Detail(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(product);
        }
        return NotFound();
    }
}
