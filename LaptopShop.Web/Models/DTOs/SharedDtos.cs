namespace LaptopShop.Web.Models.DTOs;

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public string DetailDesc { get; set; }
    public string ShortDesc { get; set; }
    public long Quantity { get; set; }
    public long Sold { get; set; }
    public string Factory { get; set; }
    public string Target { get; set; }
}

public class OrderDto
{
    public long Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string ReceiverName { get; set; }
    public string ReceiverAddress { get; set; }
    public string ReceiverPhone { get; set; }
    public string Status { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
}

public class OrderDetailDto
{
    public long Id { get; set; }
    public long Quantity { get; set; }
    public decimal Price { get; set; }
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
}

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
