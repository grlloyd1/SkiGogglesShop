using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SkiGogglesShop.Controllers;
using SkiGogglesShop.Models;
using SkiGogglesShop.Tests.Helpers;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task Index_ReturnsAllProducts_WhenNoFilter()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index(null, null);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ProductListViewModel>().Subject;
        model.Products.Should().HaveCount(4);
    }

    [Fact]
    public async Task Index_FiltersProductsByCategory()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index("Budget", null);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ProductListViewModel>().Subject;
        model.Products.Should().HaveCount(2);
        model.Products.Should().OnlyContain(p => p.Category == "Budget");
        model.SelectedCategory.Should().Be("Budget");
    }

    [Fact]
    public async Task Index_FiltersProductsByLensColor()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index(null, "Blue");

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ProductListViewModel>().Subject;
        model.Products.Should().HaveCount(1);
        model.Products.First().LensColor.Should().Be("Blue");
        model.SelectedLensColor.Should().Be("Blue");
    }

    [Fact]
    public async Task Index_CombinesFilters()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index("Budget", "Clear");

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ProductListViewModel>().Subject;
        model.Products.Should().HaveCount(1);
        model.Products.First().Name.Should().Be("Budget Goggles");
    }

    [Fact]
    public async Task Index_PopulatesCategoriesAndLensColors()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Index(null, null);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<ProductListViewModel>().Subject;
        model.Categories.Should().Contain(new[] { "Budget", "Mid-Range", "Premium" });
        model.LensColors.Should().Contain(new[] { "Clear", "Blue", "Mirrored", "Orange" });
    }

    [Fact]
    public async Task Details_ReturnsProduct_WhenValidId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var product = viewResult.Model.Should().BeOfType<Product>().Subject;
        product.Id.Should().Be(1);
        product.Name.Should().Be("Budget Goggles");
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenInvalidId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Details(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenNullId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new ProductsController(context);

        // Act
        var result = await controller.Details(null);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
