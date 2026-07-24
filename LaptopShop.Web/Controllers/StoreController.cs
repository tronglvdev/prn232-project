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
        var filters = new List<string>();

        if (!string.IsNullOrEmpty(name))
            filters.Add($"contains(tolower(Name), '{name.ToLower()}')");

        if (factory != null && factory.Length > 0)
        {
            var fFilters = factory.Select(f => $"Factory eq '{f}'");
            filters.Add($"({string.Join(" or ", fFilters)})");
        }

        if (target != null && target.Length > 0)
        {
            var tFilters = target.Select(t => $"Target eq '{t}'");
            filters.Add($"({string.Join(" or ", tFilters)})");
        }

        if (price != null && price.Length > 0)
        {
            var pFilters = new List<string>();
            foreach (var p in price)
            {
                if (p == "duoi-10-trieu") pFilters.Add("Price lt 10000000");
                else if (p == "10-15-trieu") pFilters.Add("(Price ge 10000000 and Price le 15000000)");
                else if (p == "15-20-trieu") pFilters.Add("(Price ge 15000000 and Price le 20000000)");
                else if (p == "tren-20-trieu") pFilters.Add("Price gt 20000000");
            }
            if (pFilters.Any()) filters.Add($"({string.Join(" or ", pFilters)})");
        }

        var odataQuery = new List<string>();
        if (filters.Any()) odataQuery.Add($"$filter={string.Join(" and ", filters)}");

        if (!string.IsNullOrEmpty(sort))
        {
            if (sort == "gia-tang-dan") odataQuery.Add("$orderby=Price asc");
            if (sort == "gia-giam-dan") odataQuery.Add("$orderby=Price desc");
        }

        var url = "Products" + (odataQuery.Count > 0 ? "?" + string.Join("&", odataQuery) : "");

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
