using SkiGogglesShop.Models;

namespace SkiGogglesShop.ViewModels;

public class ProductListViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public string? SelectedCategory { get; set; }
    public string? SelectedLensColor { get; set; }
    public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> LensColors { get; set; } = Enumerable.Empty<string>();
}
