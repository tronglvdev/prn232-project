using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopShop.DAL.Entities;

[Table("order_detail")]
public class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public long OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    public long ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}
