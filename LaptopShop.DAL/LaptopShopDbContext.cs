using Microsoft.EntityFrameworkCore;
using LaptopShop.DAL.Entities;

namespace LaptopShop.DAL;

public class LaptopShopDbContext : DbContext
{
    public LaptopShopDbContext(DbContextOptions<LaptopShopDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartDetail> CartDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Administrator" },
            new Role { Id = 2, Name = "User", Description = "Normal User" },
            new Role { Id = 3, Name = "Staff", Description = "Staff Member" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Asus Gaming ROG", Price = 20000000, Image = "1711078092373-asus-01.png", Factory = "ASUS", Target = "GAMING", ShortDesc = "Laptop Gaming ASUS ROG", DetailDesc = "Laptop Gaming ASUS ROG cấu hình mạnh", Quantity = 100, Sold = 10 },
            new Product { Id = 2, Name = "Dell XPS 13", Price = 25000000, Image = "1711078452562-dell-01.png", Factory = "DELL", Target = "DOANH-NHAN", ShortDesc = "Laptop Doanh nhân cao cấp", DetailDesc = "Dell XPS 13 viền siêu mỏng", Quantity = 50, Sold = 5 },
            new Product { Id = 3, Name = "Lenovo Ideapad", Price = 12000000, Image = "1711079073759-lenovo-01.png", Factory = "LENOVO", Target = "SINHVIEN-VANPHONG", ShortDesc = "Laptop học tập làm việc", DetailDesc = "Lenovo Ideapad giá rẻ", Quantity = 200, Sold = 50 },
            new Product { Id = 4, Name = "Asus Zenbook Duo", Price = 18000000, Image = "1711079496409-asus-02.png", Factory = "ASUS", Target = "THIET-KE-DO-HOA", ShortDesc = "Laptop thiết kế 2 màn hình", DetailDesc = "Zenbook Duo tuyệt đỉnh", Quantity = 30, Sold = 2 },
            new Product { Id = 5, Name = "Macbook Air M1", Price = 28000000, Image = "1711079954090-apple-01.png", Factory = "APPLE", Target = "MONG-NHE", ShortDesc = "Macbook siêu mỏng", DetailDesc = "Macbook Air M1 tiết kiệm pin", Quantity = 150, Sold = 40 },
            new Product { Id = 6, Name = "LG Gram 14", Price = 22000000, Image = "1711080386941-lg-01.png", Factory = "LG", Target = "MONG-NHE", ShortDesc = "Laptop nhẹ nhất", DetailDesc = "LG Gram siêu nhẹ", Quantity = 80, Sold = 15 },
            new Product { Id = 7, Name = "Macbook Pro M2", Price = 30000000, Image = "1711080787179-apple-02.png", Factory = "APPLE", Target = "DOANH-NHAN", ShortDesc = "Laptop Pro mạnh mẽ", DetailDesc = "Macbook Pro hiệu năng cao", Quantity = 60, Sold = 20 },
            new Product { Id = 8, Name = "Acer Nitro 5", Price = 16000000, Image = "1711080948771-acer-01.png", Factory = "ACER", Target = "GAMING", ShortDesc = "Laptop Gaming giá tốt", DetailDesc = "Acer Nitro 5 tản nhiệt mát", Quantity = 120, Sold = 25 },
            new Product { Id = 9, Name = "Asus Vivobook", Price = 14000000, Image = "1711081080930-asus-03.png", Factory = "ASUS", Target = "SINHVIEN-VANPHONG", ShortDesc = "Laptop mỏng đẹp", DetailDesc = "Asus Vivobook trẻ trung", Quantity = 150, Sold = 30 },
            new Product { Id = 10, Name = "Dell Inspiron", Price = 9000000, Image = "1711081278418-dell-02.png", Factory = "DELL", Target = "MONG-NHE", ShortDesc = "Laptop Dell giá rẻ", DetailDesc = "Dell Inspiron cho mọi nhà", Quantity = 250, Sold = 100 }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FullName = "Quản trị viên", Email = "admin@gmail.com", Password = "$2a$11$vsw8SPljVgZbM625IIPvMeT5TldMi4WlFY1VNxKfXR4DebXIlLpyG", RoleId = 1, Phone = "0123456789", Address = "Hà Nội", Avatar = "default.jpg" },
            new User { Id = 2, FullName = "Khách hàng", Email = "user@gmail.com", Password = "$2a$11$vsw8SPljVgZbM625IIPvMeT5TldMi4WlFY1VNxKfXR4DebXIlLpyG", RoleId = 2, Phone = "0987654321", Address = "Hồ Chí Minh", Avatar = "default.jpg" }
        );
    }
}
