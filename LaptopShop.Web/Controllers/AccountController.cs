using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace LaptopShop.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var loginDto = new { Email = email, Password = password };
            var response = await client.PostAsJsonAsync("Users/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = System.Text.Json.JsonSerializer.Deserialize<LaptopShop.Web.Models.DTOs.UserDto>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("Avatar", $"/images/avatar/{user.Avatar}"),
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "ADMIN" : "USER")
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                if (user.RoleId == 1)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác.";
        }
        else
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
        }
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(LaptopShop.Web.ViewModels.RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var userCreateDto = new LaptopShop.Web.Models.DTOs.UserCreateDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                RoleId = 2, // 2 = USER
                Avatar = "default.jpg",
                Phone = "",
                Address = ""
            };
            var response = await client.PostAsJsonAsync("Users", userCreateDto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Đăng ký không thành công. Email có thể đã tồn tại.";
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }
}
