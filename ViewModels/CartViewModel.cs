using SkiGogglesShop.Models;

namespace SkiGogglesShop.ViewModels;

public class CartViewModel
{
    public IEnumerable<CartItem> Items { get; set; } = Enumerable.Empty<CartItem>();
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int ItemCount => Items.Sum(i => i.Quantity);
}
