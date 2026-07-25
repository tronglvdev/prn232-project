using LaptopShop.BLL.DTOs;
using LaptopShop.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LaptopShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public UsersController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _userService.CreateUserAsync(userDto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleId == 1 ? "ADMIN" : "USER")
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            User = user
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _userService.UpdateUserAsync(id, userDto);
        return NoContent();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
