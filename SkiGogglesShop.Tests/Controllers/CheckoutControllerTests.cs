using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SkiGogglesShop.Controllers;
using SkiGogglesShop.Models;
using SkiGogglesShop.Tests.Helpers;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Tests.Controllers;

public class CheckoutControllerTests
{
    private const string TestSessionId = "test-session-123";

    [Fact]
    public async Task Index_RedirectsToCart_WhenNoSession()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext()
        };

        // Act
        var result = await controller.Index();

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Cart");
    }

    [Fact]
    public async Task Index_RedirectsToCart_WhenCartEmpty()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Index();

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Cart");
    }

    [Fact]
    public async Task Index_ReturnsCheckoutView_WhenCartHasItems()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        });
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<CheckoutViewModel>().Subject;
        model.CartItems.Should().HaveCount(1);
    }

    [Fact]
    public async Task PlaceOrder_CreatesOrder_WhenValid()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        });
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        var model = new CheckoutViewModel
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        var result = await controller.PlaceOrder(model);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Confirmation");

        context.Orders.Should().HaveCount(1);
        var order = context.Orders.First();
        order.CustomerName.Should().Be("John Doe");
        order.Email.Should().Be("john@example.com");
        order.Status.Should().Be("Confirmed");
    }

    [Fact]
    public async Task PlaceOrder_UpdatesStock()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var initialStock = context.Products.First(p => p.Id == 1).StockQuantity;

        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        });
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        var model = new CheckoutViewModel
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        await controller.PlaceOrder(model);

        // Assert
        var product = context.Products.First(p => p.Id == 1);
        product.StockQuantity.Should().Be(initialStock - 2);
    }

    [Fact]
    public async Task PlaceOrder_ClearsCart()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        });
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        var model = new CheckoutViewModel
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        await controller.PlaceOrder(model);

        // Assert
        context.CartItems.Where(c => c.SessionId == TestSessionId).Should().BeEmpty();
    }

    [Fact]
    public async Task PlaceOrder_CalculatesCorrectTotal()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1, // $59.99
            Quantity = 2
        });
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        var model = new CheckoutViewModel
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St"
        };

        // Act
        await controller.PlaceOrder(model);

        // Assert
        var order = context.Orders.First();
        order.TotalAmount.Should().Be(59.99m * 2);
    }

    [Fact]
    public async Task Confirmation_ReturnsOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var order = new Order
        {
            CustomerName = "John Doe",
            Email = "john@example.com",
            ShippingAddress = "123 Main St",
            TotalAmount = 100m,
            Status = "Confirmed"
        };
        context.Orders.Add(order);
        context.SaveChanges();

        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext()
        };

        // Act
        var result = await controller.Confirmation(order.Id);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<Order>().Subject;
        model.Id.Should().Be(order.Id);
        model.CustomerName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Confirmation_ReturnsNotFound_WhenInvalidId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CheckoutController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext()
        };

        // Act
        var result = await controller.Confirmation(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
