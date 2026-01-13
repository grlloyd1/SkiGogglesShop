using SkiGogglesShop.Models;

namespace SkiGogglesShop.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Products.Any())
        {
            return;
        }

        var products = new Product[]
        {
            // Budget options ($50-80)
            new Product
            {
                Name = "Alpine Basic",
                Description = "Entry-level ski goggles with anti-fog coating and UV protection. Perfect for beginners.",
                Price = 54.99m,
                ImageUrl = "/images/goggles/alpine-basic.jpg",
                Category = "Budget",
                LensColor = "Clear",
                FrameStyle = "Full Frame",
                StockQuantity = 25
            },
            new Product
            {
                Name = "Snowview Orange",
                Description = "Budget-friendly goggles with orange lens for low-light conditions. Great visibility on cloudy days.",
                Price = 59.99m,
                ImageUrl = "/images/goggles/snowview-orange.jpg",
                Category = "Budget",
                LensColor = "Orange",
                FrameStyle = "Full Frame",
                StockQuantity = 30
            },
            new Product
            {
                Name = "Glacier Entry",
                Description = "Comfortable fit with adjustable strap. Double-layer foam for all-day comfort.",
                Price = 74.99m,
                ImageUrl = "/images/goggles/glacier-entry.jpg",
                Category = "Budget",
                LensColor = "Yellow",
                FrameStyle = "Full Frame",
                StockQuantity = 20
            },

            // Mid-range options ($100-150)
            new Product
            {
                Name = "ProVision Blue",
                Description = "Enhanced contrast blue lens with triple-layer foam. Excellent peripheral vision.",
                Price = 119.99m,
                ImageUrl = "/images/goggles/provision-blue.jpg",
                Category = "Mid-Range",
                LensColor = "Blue",
                FrameStyle = "Semi-Frameless",
                StockQuantity = 15
            },
            new Product
            {
                Name = "Summit Mirror",
                Description = "Mirrored lens reduces glare on bright days. Helmet-compatible design.",
                Price = 134.99m,
                ImageUrl = "/images/goggles/summit-mirror.jpg",
                Category = "Mid-Range",
                LensColor = "Mirrored Silver",
                FrameStyle = "Frameless",
                StockQuantity = 18
            },
            new Product
            {
                Name = "ClearView Pro",
                Description = "Interchangeable lens system with clear and tinted options included.",
                Price = 149.99m,
                ImageUrl = "/images/goggles/clearview-pro.jpg",
                Category = "Mid-Range",
                LensColor = "Clear/Rose",
                FrameStyle = "Full Frame",
                StockQuantity = 12
            },
            new Product
            {
                Name = "AllWeather Sport",
                Description = "Photochromic lens adapts to changing light conditions automatically.",
                Price = 139.99m,
                ImageUrl = "/images/goggles/allweather-sport.jpg",
                Category = "Mid-Range",
                LensColor = "Photochromic",
                FrameStyle = "Semi-Frameless",
                StockQuantity = 10
            },

            // Premium options ($200-300)
            new Product
            {
                Name = "Elite Spherical",
                Description = "Premium spherical lens for distortion-free vision. Magnetic quick-change system.",
                Price = 229.99m,
                ImageUrl = "/images/goggles/elite-spherical.jpg",
                Category = "Premium",
                LensColor = "Rose Gold Mirror",
                FrameStyle = "Frameless",
                StockQuantity = 8
            },
            new Product
            {
                Name = "Pro-X Competition",
                Description = "Competition-grade goggles used by professional athletes. Ultimate clarity and comfort.",
                Price = 279.99m,
                ImageUrl = "/images/goggles/pro-x-competition.jpg",
                Category = "Premium",
                LensColor = "Blue Mirror",
                FrameStyle = "Frameless",
                StockQuantity = 6
            },
            new Product
            {
                Name = "Titanium Series",
                Description = "Titanium-reinforced frame with heated lens technology. The ultimate in ski eyewear.",
                Price = 299.99m,
                ImageUrl = "/images/goggles/titanium-series.jpg",
                Category = "Premium",
                LensColor = "Mirrored Gold",
                FrameStyle = "Full Frame",
                StockQuantity = 5
            },
            new Product
            {
                Name = "Nordic Vision",
                Description = "Scandinavian design with ultra-wide field of view. Premium anti-scratch coating.",
                Price = 249.99m,
                ImageUrl = "/images/goggles/nordic-vision.jpg",
                Category = "Premium",
                LensColor = "Green Mirror",
                FrameStyle = "Frameless",
                StockQuantity = 7
            },
            new Product
            {
                Name = "Stealth Carbon",
                Description = "Carbon fiber accents with matte black finish. Includes two interchangeable lenses.",
                Price = 269.99m,
                ImageUrl = "/images/goggles/stealth-carbon.jpg",
                Category = "Premium",
                LensColor = "Smoke/Clear",
                FrameStyle = "Semi-Frameless",
                StockQuantity = 9
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
