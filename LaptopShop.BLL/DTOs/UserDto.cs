namespace LaptopShop.BLL.DTOs;

public class UserDto
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Avatar { get; set; }
    public long RoleId { get; set; }
    public string RoleName { get; set; }
}

public class UserCreateDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public long RoleId { get; set; }
    public string Avatar { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
