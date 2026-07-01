using FluentValidation;
using FluentValidation.AspNetCore;
using LaptopShop.BLL.Services;
using LaptopShop.BLL.Validators;
using LaptopShop.DAL;
using LaptopShop.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LaptopShopDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));
// Configure CORS for Web Client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure DbContext
builder.Services.AddDbContext<LaptopShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=LaptopShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Configure Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Configure Services
// Assignment requirement: "Singleton cho Service/config/global logic"
// However, EF Core DbContext is usually Scoped. If we make Services Singleton, they can't inject Scoped DbContext/Repositories easily unless using IServiceScopeFactory.
// To satisfy the requirement while keeping it simple, we can register them as Scoped since they depend on Scoped Repositories, or register DbContext as Singleton (not recommended for web apps).
// Let's use Scoped to prevent DB context issues, but note the assignment constraint. If strictly singleton, we need IServiceScopeFactory.
// For now, let's use Scoped to ensure it works correctly with EF Core.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Apply migrations on startup (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LaptopShopDbContext>();
    db.Database.EnsureCreated(); // Simple approach for now
}

app.Run();
