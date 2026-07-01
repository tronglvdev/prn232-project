using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopShop.DAL.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [StringLength(255)]
    public string ReceiverName { get; set; }

    [StringLength(500)]
    public string ReceiverAddress { get; set; }

    [StringLength(20)]
    public string ReceiverPhone { get; set; }

    [StringLength(50)]
    public string Status { get; set; } // Pending, Shipping, Delivered, ReturnRequested, Returned

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
