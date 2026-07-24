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

        var deliveredOrders = orders?.Where(o => o.Status == "Delivered").ToList();
        ViewBag.TotalRevenue = deliveredOrders?.Sum(o => o.TotalPrice) ?? 0;

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


    [HttpPost("Admin/Product/Update/{id}")]
    public async Task<IActionResult> ProductUpdate(long id, ProductDto product, IFormFile? hoidanitFile)
    {
        product.Id = id;

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
            TempData["Message"] = "Cập nhật sản phẩm thành công.";
        }
        else
        {
            TempData["Message"] = "Cập nhật sản phẩm thất bại.";
        }
        return RedirectToAction("Product");
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
            TempData["Message"] = "Thêm sản phẩm thành công.";
        }
        else
        {
            TempData["Message"] = "Thêm sản phẩm thất bại.";
        }
        return RedirectToAction("Product");
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

    [HttpPost("Admin/Order/UpdateStatus/{id}")]
    public async Task<IActionResult> OrderUpdateStatus(long id, string status)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        HttpResponseMessage response;

        if (status == "ReturnApproved")
        {
            response = await client.PostAsync($"Orders/{id}/approve-return", null);
        }
        else if (status == "Cancelled")
        {
            response = await client.PutAsJsonAsync($"Orders/{id}/status", "Cancelled");
        }
        else
        {
            response = await client.PutAsJsonAsync($"Orders/{id}/status", status);
        }

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Cập nhật trạng thái đơn hàng thành công.";
        }
        else
        {
            TempData["Message"] = "Cập nhật trạng thái đơn hàng thất bại.";
        }
        return RedirectToAction("Order");
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

    [HttpPost("Admin/User/Update/{id}")]
    public async Task<IActionResult> UserUpdate(long id, UserCreateDto user, IFormFile? avatarFile)
    {
        user.Avatar = "avatar.jpg";
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"Users/{id}", user);
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Cập nhật người dùng thành công.";
        }
        else
        {
            TempData["Message"] = "Cập nhật người dùng thất bại.";
        }
        return RedirectToAction("User");
    }

    [HttpGet("Admin/User/Delete/{id}")]
    public async Task<IActionResult> UserDelete(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"Users/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Xóa người dùng thành công.";
        }
        else
        {
            TempData["Message"] = "Xóa người dùng thất bại.";
        }
        return RedirectToAction("User");
    }

    [HttpPost("Admin/User/Create")]
    public async Task<IActionResult> UserCreate(UserCreateDto user, IFormFile? avatarFile)
    {
        user.Avatar = "avatar.jpg";
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("Users", user);
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Thêm người dùng thành công.";
        }
        else
        {
            TempData["Message"] = "Thêm người dùng thất bại.";
        }
        return RedirectToAction("User");
    }
}
