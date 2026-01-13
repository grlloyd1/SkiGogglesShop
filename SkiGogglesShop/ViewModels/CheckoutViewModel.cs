using System.ComponentModel.DataAnnotations;
using SkiGogglesShop.Models;

namespace SkiGogglesShop.ViewModels;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Shipping address is required")]
    [StringLength(500)]
    [Display(Name = "Shipping Address")]
    public string ShippingAddress { get; set; } = string.Empty;

    public IEnumerable<CartItem> CartItems { get; set; } = Enumerable.Empty<CartItem>();
    public decimal Total => CartItems.Sum(i => i.Subtotal);
}
