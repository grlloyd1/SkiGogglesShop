using FluentAssertions;
using SkiGogglesShop.Data;
using SkiGogglesShop.Tests.Helpers;

namespace SkiGogglesShop.Tests.Data;

public class DbInitializerTests
{
    [Fact]
    public void Initialize_SeedsProducts_WhenDatabaseEmpty()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        context.Products.Should().BeEmpty();

        // Act
        DbInitializer.Initialize(context);

        // Assert
        context.Products.Should().NotBeEmpty();
        context.Products.Should().HaveCountGreaterThan(10); // We seed 12 products
    }

    [Fact]
    public void Initialize_DoesNotDuplicate_WhenProductsExist()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        DbInitializer.Initialize(context);
        var initialCount = context.Products.Count();

        // Act
        DbInitializer.Initialize(context);

        // Assert
        context.Products.Should().HaveCount(initialCount);
    }

    [Fact]
    public void Initialize_SeedsAllCategories()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();

        // Act
        DbInitializer.Initialize(context);

        // Assert
        var categories = context.Products.Select(p => p.Category).Distinct().ToList();
        categories.Should().Contain(new[] { "Budget", "Mid-Range", "Premium" });
    }

    [Fact]
    public void Initialize_SeedsProductsWithAllRequiredFields()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();

        // Act
        DbInitializer.Initialize(context);

        // Assert
        context.Products.Should().OnlyContain(p =>
            !string.IsNullOrEmpty(p.Name) &&
            !string.IsNullOrEmpty(p.Description) &&
            p.Price > 0 &&
            !string.IsNullOrEmpty(p.Category) &&
            !string.IsNullOrEmpty(p.LensColor) &&
            !string.IsNullOrEmpty(p.FrameStyle) &&
            p.StockQuantity >= 0
        );
    }
}
