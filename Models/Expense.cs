using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class Expense
{
    public ulong ExpenseId { get; set; }

    [Required(ErrorMessage = "Expense title is required.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    public DateTime ExpenseDate { get; set; } = DateTime.Now;

    [StringLength(60)]
    public string? ReferenceNumber { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}