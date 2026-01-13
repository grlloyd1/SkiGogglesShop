using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiGogglesShop.Data;
using SkiGogglesShop.ViewModels;

namespace SkiGogglesShop.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? category, string? lensColor)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (!string.IsNullOrEmpty(lensColor))
        {
            query = query.Where(p => p.LensColor == lensColor);
        }

        var viewModel = new ProductListViewModel
        {
            Products = await query.ToListAsync(),
            SelectedCategory = category,
            SelectedLensColor = lensColor,
            Categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync(),
            LensColors = await _context.Products.Select(p => p.LensColor).Distinct().ToListAsync()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
