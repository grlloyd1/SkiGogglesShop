using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiGogglesShop.Data;
using SkiGogglesShop.Models;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Controllers;

public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string SessionKeyName = "CartSessionId";

    public CheckoutController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string? GetSessionId()
    {
        return HttpContext.Session.GetString(SessionKeyName);
    }

    public async Task<IActionResult> Index()
    {
        var sessionId = GetSessionId();
        if (string.IsNullOrEmpty(sessionId))
        {
            return RedirectToAction("Index", "Cart");
        }

        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.SessionId == sessionId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var viewModel = new CheckoutViewModel
        {
            CartItems = cartItems
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var sessionId = GetSessionId();
        if (string.IsNullOrEmpty(sessionId))
        {
            return RedirectToAction("Index", "Cart");
        }

        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.SessionId == sessionId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        model.CartItems = cartItems;

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var order = new Order
        {
            CustomerName = model.CustomerName,
            Email = model.Email,
            ShippingAddress = model.ShippingAddress,
            OrderDate = DateTime.UtcNow,
            TotalAmount = cartItems.Sum(c => c.Subtotal),
            Status = "Confirmed"
        };

        foreach (var item in cartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            });

            // Update stock
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
            }
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        // Clear session
        HttpContext.Session.Remove(SessionKeyName);

        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
