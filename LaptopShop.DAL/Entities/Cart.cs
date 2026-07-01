using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopShop.DAL.Entities;

[Table("carts")]
public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public int Sum { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
}
