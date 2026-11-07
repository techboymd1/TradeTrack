using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class Sale
{
    public ulong SaleId { get; set; }

    [Required]
    [StringLength(40)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public ulong? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Completed";

    public DateTime CreatedAt { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}