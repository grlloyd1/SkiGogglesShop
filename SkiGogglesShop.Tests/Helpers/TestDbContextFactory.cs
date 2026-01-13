using Microsoft.EntityFrameworkCore;
using SkiGogglesShop.Data;
using SkiGogglesShop.Models;

namespace SkiGogglesShop.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static ApplicationDbContext CreateWithProducts(string? databaseName = null)
    {
        var context = Create(databaseName);
        SeedProducts(context);
        return context;
    }

    public static void SeedProducts(ApplicationDbContext context)
    {
        if (context.Products.Any()) return;

        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Budget Goggles",
                Description = "Entry level goggles",
                Price = 59.99m,
                Category = "Budget",
                LensColor = "Clear",
                FrameStyle = "Full Frame",
                StockQuantity = 10
            },
            new Product
            {
                Id = 2,
                Name = "Mid-Range Goggles",
                Description = "Quality goggles",
                Price = 129.99m,
                Category = "Mid-Range",
                LensColor = "Blue",
                FrameStyle = "Frameless",
                StockQuantity = 5
            },
            new Product
            {
                Id = 3,
                Name = "Premium Goggles",
                Description = "Top of the line",
                Price = 299.99m,
                Category = "Premium",
                LensColor = "Mirrored",
                FrameStyle = "Frameless",
                StockQuantity = 3
            },
            new Product
            {
                Id = 4,
                Name = "Another Budget",
                Description = "Another budget option",
                Price = 49.99m,
                Category = "Budget",
                LensColor = "Orange",
                FrameStyle = "Full Frame",
                StockQuantity = 0  // Out of stock
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
