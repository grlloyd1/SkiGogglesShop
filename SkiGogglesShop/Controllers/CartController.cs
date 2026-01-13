using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiGogglesShop.Data;
using SkiGogglesShop.Models;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string SessionKeyName = "CartSessionId";

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetOrCreateSessionId()
    {
        var sessionId = HttpContext.Session.GetString(SessionKeyName);
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(SessionKeyName, sessionId);
        }
        return sessionId;
    }

    public async Task<IActionResult> Index()
    {
        var sessionId = GetOrCreateSessionId();
        var items = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.SessionId == sessionId)
            .ToListAsync();

        var viewModel = new CartViewModel
        {
            Items = items
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var sessionId = GetOrCreateSessionId();
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                SessionId = sessionId,
                ProductId = productId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        TempData["Message"] = $"{product.Name} added to cart!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, int quantity)
    {
        var sessionId = GetOrCreateSessionId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.SessionId == sessionId);

        if (item == null)
        {
            return NotFound();
        }

        if (quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var sessionId = GetOrCreateSessionId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.SessionId == sessionId);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<int> GetCartCount()
    {
        var sessionId = HttpContext.Session.GetString(SessionKeyName);
        if (string.IsNullOrEmpty(sessionId))
        {
            return 0;
        }

        return await _context.CartItems
            .Where(c => c.SessionId == sessionId)
            .SumAsync(c => c.Quantity);
    }
}
