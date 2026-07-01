using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopShop.DAL.Entities;

[Table("products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(255)]
    public string Image { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string DetailDesc { get; set; }

    [StringLength(1000)]
    public string ShortDesc { get; set; }

    public long Quantity { get; set; }

    public long Sold { get; set; }

    [StringLength(255)]
    public string Factory { get; set; }

    [StringLength(255)]
    public string Target { get; set; }
}
