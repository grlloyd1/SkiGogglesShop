using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SkiGogglesShop.Controllers;
using SkiGogglesShop.Models;
using SkiGogglesShop.Tests.Helpers;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Tests.Controllers;

public class CartControllerTests
{
    private const string TestSessionId = "test-session-123";

    [Fact]
    public async Task Index_ReturnsEmptyCart_WhenNoSession()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext()
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<CartViewModel>().Subject;
        model.Items.Should().BeEmpty();
        model.Total.Should().Be(0);
    }

    [Fact]
    public async Task Index_ReturnsCartItems_WhenItemsExist()
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

        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<CartViewModel>().Subject;
        model.Items.Should().HaveCount(1);
        model.ItemCount.Should().Be(2);
    }

    [Fact]
    public async Task Add_CreatesNewCartItem_WhenProductNotInCart()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId),
            TempData = MockHttpContext.CreateTempData()
        };

        // Act
        var result = await controller.Add(1, 2);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        context.CartItems.Should().HaveCount(1);
        var cartItem = context.CartItems.First();
        cartItem.ProductId.Should().Be(1);
        cartItem.Quantity.Should().Be(2);
    }

    [Fact]
    public async Task Add_IncrementsQuantity_WhenProductAlreadyInCart()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        context.CartItems.Add(new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 1
        });
        context.SaveChanges();

        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId),
            TempData = MockHttpContext.CreateTempData()
        };

        // Act
        var result = await controller.Add(1, 2);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        context.CartItems.Should().HaveCount(1);
        context.CartItems.First().Quantity.Should().Be(3);
    }

    [Fact]
    public async Task Add_ReturnsNotFound_WhenInvalidProductId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Add(999, 1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ChangesQuantity()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var cartItem = new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        };
        context.CartItems.Add(cartItem);
        context.SaveChanges();

        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Update(cartItem.Id, 5);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        context.CartItems.First().Quantity.Should().Be(5);
    }

    [Fact]
    public async Task Update_RemovesItem_WhenQuantityZeroOrLess()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var cartItem = new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        };
        context.CartItems.Add(cartItem);
        context.SaveChanges();

        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Update(cartItem.Id, 0);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        context.CartItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Remove_DeletesCartItem()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var cartItem = new CartItem
        {
            SessionId = TestSessionId,
            ProductId = 1,
            Quantity = 2
        };
        context.CartItems.Add(cartItem);
        context.SaveChanges();

        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Remove(cartItem.Id);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        context.CartItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Remove_DoesNothing_WhenItemNotFound()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateWithProducts();
        var controller = new CartController(context)
        {
            ControllerContext = MockHttpContext.CreateControllerContext(TestSessionId)
        };

        // Act
        var result = await controller.Remove(999);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
    }
}
