using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkiGogglesShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [StringLength(50)]
    public string LensColor { get; set; } = string.Empty;

    [StringLength(50)]
    public string FrameStyle { get; set; } = string.Empty;

    public int StockQuantity { get; set; }

    public bool IsAvailable => StockQuantity > 0;
}
