using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class Sale
{
    public ulong SaleId { get; set; }

    [Required]
    [StringLength(40)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal Subtotal { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal DiscountAmount { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Completed";

    public DateTime CreatedAt { get; set; }

    public ICollection<SaleItem> Items { get; set; }
        = new List<SaleItem>();
}