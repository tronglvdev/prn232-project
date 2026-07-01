using LaptopShop.BLL.DTOs;
using LaptopShop.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaptopShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _userService.CreateUserAsync(userDto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _userService.UpdateUserAsync(id, userDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
