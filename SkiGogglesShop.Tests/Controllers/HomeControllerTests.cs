using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SkiGogglesShop.Controllers;
using SkiGogglesShop.Models;
using SkiGogglesShop.Tests.Helpers;

namespace SkiGogglesShop.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
    }

    [Fact]
    public async Task Index_ReturnsViewWithFeaturedProducts()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new HomeController(_mockLogger.Object, context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Product>>().Subject;
        model.Should().HaveCount(3); // Top 3 featured products
    }

    [Fact]
    public async Task Index_ReturnsProductsOrderedByPriceDescending()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new HomeController(_mockLogger.Object, context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var products = viewResult.Model.Should().BeAssignableTo<IEnumerable<Product>>().Subject.ToList();

        products.Should().BeInDescendingOrder(p => p.Price);
        products.First().Name.Should().Be("Premium Goggles"); // Highest price
    }

    [Fact]
    public async Task Index_ExcludesOutOfStockProducts()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new HomeController(_mockLogger.Object, context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var products = viewResult.Model.Should().BeAssignableTo<IEnumerable<Product>>().Subject;

        products.Should().OnlyContain(p => p.StockQuantity > 0);
    }

    [Fact]
    public void Privacy_ReturnsView()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var controller = new HomeController(_mockLogger.Object, context);

        // Act
        var result = controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
}
