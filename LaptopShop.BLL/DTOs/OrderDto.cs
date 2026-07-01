namespace LaptopShop.BLL.DTOs;

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
