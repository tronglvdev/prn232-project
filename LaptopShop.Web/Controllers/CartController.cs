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
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Products/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var item = cart.FirstOrDefault(c => c.ProductId == id);
            int currentQty = item != null ? item.Quantity : 0;

            if (currentQty + quantity > product.Quantity)
            {
                TempData["Error"] = $"Không đủ số lượng trong kho. (Còn lại: {product.Quantity})";
                return RedirectToAction("Index");
            }

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Quantity = quantity
                });
            }
            SaveCart(cart);
        }

        return RedirectToAction("Index");
    }

    [HttpPost("/api/add-product-to-cart")]
    public async Task<IActionResult> AddToCartAjax([FromBody] AddToCartRequest request)
    {
        var cart = GetCart();
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"Products/{request.ProductId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var item = cart.FirstOrDefault(c => c.ProductId == request.ProductId);
            int currentQty = item != null ? item.Quantity : 0;

            if (currentQty + request.Quantity > product.Quantity)
            {
                return BadRequest(new { message = $"Sản phẩm chỉ còn {product.Quantity} chiếc trong kho." });
            }

            if (item != null)
            {
                item.Quantity += request.Quantity;
            }
            else
            {
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

        foreach (var item in cart)
        {
            var responseProduct = await client.GetAsync($"Products/{item.ProductId}");
            if (responseProduct.IsSuccessStatusCode)
            {
                var content = await responseProduct.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (item.Quantity > product.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm '{item.ProductName}' chỉ còn {product.Quantity} chiếc trong kho. Vui lòng cập nhật lại giỏ hàng.";
                    return RedirectToAction("Index");
                }
            }
        }

        long userId = 1;
        string userName = "Khách";
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

    [HttpPost]
    public async Task<IActionResult> CancelOrder(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"Orders/{id}/status", "Cancelled");
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Huỷ đơn hàng thành công!";
        }
        return RedirectToAction("History");
    }

    [HttpPost]
    public async Task<IActionResult> RequestReturn(long id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsync($"Orders/{id}/request-return", null);
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Đã gửi yêu cầu trả hàng, vui lòng chờ Admin duyệt!";
        }
        return RedirectToAction("History");
    }
}
