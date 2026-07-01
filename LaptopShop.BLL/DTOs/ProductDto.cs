namespace LaptopShop.BLL.DTOs;

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
