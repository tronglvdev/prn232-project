using System.Text.Json;
using LaptopShop.Web.Models;
using LaptopShop.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LaptopShop.Web.Controllers;

public class AddToCartRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

[Authorize]
public class CartController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CartController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private List<CartItem> GetCart()
    {
        var sessionCart = HttpContext.Session.GetString("Cart");
        if (sessionCart == null)
            return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(sessionCart);
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(long id, int quantity = 1)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == id);
        
        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                cart.Add(new CartItem 
                { 
                    ProductId = product.Id, 
                    ProductName = product.Name, 
                    Price = product.Price, 
                    Image = product.Image, 
                    Quantity = quantity 
                });
            }
        }
        
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    [HttpPost("/api/add-product-to-cart")]
    public async Task<IActionResult> AddToCartAjax([FromBody] AddToCartRequest request)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == request.ProductId);
        
        if (item != null)
        {
            item.Quantity += request.Quantity;
        }
        else
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"Products/{request.ProductId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                cart.Add(new CartItem 
                { 
                    ProductId = product.Id, 
                    ProductName = product.Name, 
                    Price = product.Price, 
                    Image = product.Image, 
                    Quantity = request.Quantity 
                });
            }
        }
        
        SaveCart(cart);
        int sum = cart.Sum(c => c.Quantity);
        return Ok(sum);
    }

    public IActionResult Remove(long id)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
        {
            cart.Remove(item);
            SaveCart(cart);
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (cart.Count == 0) return RedirectToAction("Index");

        var userIdClaim = User.FindFirst("Id");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long parsedId))
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"Users/{parsedId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.User = user;
            }
        }

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(string receiverName, string receiverAddress, string receiverPhone)
    {
        var cart = GetCart();
        if (cart.Count == 0) return RedirectToAction("Index");

        var client = _httpClientFactory.CreateClient("ApiClient");

        long userId = 1; // Default
        string userName = "Khách vãng lai";
        var userIdClaim = User.FindFirst("Id");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long parsedId))
        {
            userId = parsedId;
            userName = User.FindFirst("FullName")?.Value ?? "Khách hàng";
        }

        var order = new OrderDto
        {
            ReceiverName = receiverName,
            ReceiverAddress = receiverAddress,
            ReceiverPhone = receiverPhone,
            TotalPrice = cart.Sum(c => c.Price * c.Quantity),
            Status = "Pending",
            UserId = userId,
            UserName = userName,
            OrderDetails = cart.Select(c => new OrderDetailDto
            {
                ProductId = c.ProductId,
                Price = c.Price,
                Quantity = c.Quantity,
                ProductName = c.ProductName
            }).ToList()
        };

        var response = await client.PostAsJsonAsync("Orders", order);
        if (response.IsSuccessStatusCode)
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("ThankYou");
        }
        
        return View("Checkout", cart);
    }

    public IActionResult ThankYou()
    {
        return View();
    }

    public async Task<IActionResult> History()
    {
        var userIdClaim = User.FindFirst("Id");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long parsedId))
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"Orders/user/{parsedId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(orders);
            }
        }
        return View(new List<OrderDto>());
    }
}
