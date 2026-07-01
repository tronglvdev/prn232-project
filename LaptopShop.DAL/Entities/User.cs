using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopShop.DAL.Entities;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [Required]
    [StringLength(255)]
    public string FullName { get; set; }

    [StringLength(500)]
    public string Address { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(255)]
    public string Avatar { get; set; }

    public long RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Cart Cart { get; set; }
}
