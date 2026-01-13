using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkiGogglesShop.Models;

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public string SessionId { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Range(1, 100)]
    public int Quantity { get; set; }

    [NotMapped]
    public decimal Subtotal => Product?.Price * Quantity ?? 0;
}
