using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using SkiGogglesShop.Models;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Tests.Models;

public class ModelValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Product_RequiresName()
    {
        // Arrange
        var product = new Product
        {
            Name = "",
            Price = 99.99m,
            StockQuantity = 10
        };

        // Act
        var results = ValidateModel(product);

        // Assert
        results.Should().Contain(r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Product_IsValid_WhenAllRequiredFieldsPresent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Goggles",
            Description = "Test description",
            Price = 99.99m,
            Category = "Budget",
            LensColor = "Clear",
            FrameStyle = "Full Frame",
            StockQuantity = 10
        };

        // Act
        var results = ValidateModel(product);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void Order_RequiresCustomerName()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "",
            Email = "test@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        var results = ValidateModel(order);

        // Assert
        results.Should().Contain(r => r.MemberNames.Contains("CustomerName"));
    }

    [Fact]
    public void Order_RequiresValidEmail()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            Email = "invalid-email",
            ShippingAddress = "123 Main St"
        };

        // Act
        var results = ValidateModel(order);

        // Assert
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void Order_RequiresShippingAddress()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            Email = "test@example.com",
            ShippingAddress = ""
        };

        // Act
        var results = ValidateModel(order);

        // Assert
        results.Should().Contain(r => r.MemberNames.Contains("ShippingAddress"));
    }

    [Fact]
    public void CheckoutViewModel_ValidatesAllRequiredFields()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            CustomerName = "",
            Email = "",
            ShippingAddress = ""
        };

        // Act
        var results = ValidateModel(viewModel);

        // Assert
        results.Should().HaveCount(3);
        results.Should().Contain(r => r.MemberNames.Contains("CustomerName"));
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
        results.Should().Contain(r => r.MemberNames.Contains("ShippingAddress"));
    }

    [Fact]
    public void CheckoutViewModel_IsValid_WhenAllFieldsPresent()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        var results = ValidateModel(viewModel);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void Product_IsAvailable_ReturnsTrueWhenInStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 5 };

        // Assert
        product.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Product_IsAvailable_ReturnsFalseWhenOutOfStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 0 };

        // Assert
        product.IsAvailable.Should().BeFalse();
    }
}
