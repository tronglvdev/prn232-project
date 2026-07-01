using System.Net.Http;
using System.Text.Json;
using LaptopShop.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LaptopShop.Web.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var users = await client.GetFromJsonAsync<List<UserDto>>("Users");
        var products = await client.GetFromJsonAsync<List<ProductDto>>("Products");
        var orders = await client.GetFromJsonAsync<List<OrderDto>>("Orders");

        ViewBag.CountUsers = users?.Count ?? 0;
        ViewBag.CountProducts = products?.Count ?? 0;
        ViewBag.CountOrders = orders?.Count ?? 0;

        return View();
    }

    public async Task<IActionResult> Product(int page = 1)
    {
        int pageSize = 5;
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("Products");
        var products = new List<ProductDto>();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            products = JsonSerializer.Deserialize<List<ProductDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        int totalProducts = products.Count;
        int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View(pagedProducts);
    }

    [HttpGet("Admin/Product/View/{id}")]
    public async Task<IActionResult> ProductView(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(product);
        }
        return RedirectToAction("Product");
    }

    [HttpGet("Admin/Product/Update/{id}")]
    public async Task<IActionResult> ProductUpdate(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(product);
        }
        return RedirectToAction("Product");
    }

    [HttpPost("Admin/Product/Update/{id}")]
    public async Task<IActionResult> ProductUpdate(long id, ProductDto product, IFormFile? hoidanitFile)
    {
        product.Id = id; // Ensure ID matches URL

        if (hoidanitFile != null && hoidanitFile.Length > 0)
        {
            var fileName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "-" + hoidanitFile.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "product", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await hoidanitFile.CopyToAsync(stream);
            }
            product.Image = fileName;
        }

        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"Products/{id}", product);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Product");
        }

        return View(product);
    }

    [HttpGet("Admin/Product/Delete/{id}")]
    public async Task<IActionResult> ProductDelete(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"Products/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Xóa sản phẩm thành công.";
        }
        else
        {
            TempData["Message"] = "Xóa sản phẩm thất bại.";
        }
        return RedirectToAction("Product");
    }

    [HttpGet("Admin/Product/Create")]
    public IActionResult ProductCreate()
    {
        return View(new ProductDto());
    }

    [HttpPost("Admin/Product/Create")]
    public async Task<IActionResult> ProductCreate(ProductDto product, IFormFile? hoidanitFile)
    {
        if (hoidanitFile != null && hoidanitFile.Length > 0)
        {
            var fileName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "-" + hoidanitFile.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "product", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await hoidanitFile.CopyToAsync(stream);
            }
            product.Image = fileName;
        }
        else
        {
            product.Image = "default.jpg";
        }

        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("Products", product);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Product");
        }

        return View(product);
    }

    public async Task<IActionResult> Order(int page = 1)
    {
        int pageSize = 5;
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("Orders");
        var orders = new List<OrderDto>();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            orders = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        int totalOrders = orders.Count;
        int totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        var pagedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View(pagedOrders);
    }

    [ActionName("User")]
    public async Task<IActionResult> ManageUser(int page = 1)
    {
        int pageSize = 5;
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("Users");
        var users = new List<UserDto>();
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            users = JsonSerializer.Deserialize<List<UserDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        int totalUsers = users.Count;
        int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View(pagedUsers);
    }

    [HttpGet("Admin/User/View/{id}")]
    public async Task<IActionResult> UserView(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Users/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(user);
        }
        return RedirectToAction("User");
    }

    [HttpGet("Admin/User/Update/{id}")]
    public IActionResult UserUpdate(long id)
    {
        TempData["Message"] = "Tính năng Cập nhật người dùng đang được phát triển.";
        return RedirectToAction("User");
    }

    [HttpGet("Admin/User/Delete/{id}")]
    public IActionResult UserDelete(long id)
    {
        TempData["Message"] = "Tính năng Xóa người dùng đang được phát triển.";
        return RedirectToAction("User");
    }

    [HttpGet("Admin/User/Create")]
    public IActionResult UserCreate()
    {
        TempData["Message"] = "Tính năng Thêm mới người dùng đang được phát triển.";
        return RedirectToAction("User");
    }
}
