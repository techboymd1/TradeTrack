using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class Product
{
    public ulong ProductId { get; set; }

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(40)]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Category { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "unit";

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal CostPrice { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal SellingPrice { get; set; }

    [Range(typeof(decimal), "0", "999999999.999")]
    public decimal QuantityInStock { get; set; }

    [Range(typeof(decimal), "0", "999999999.999")]
    public decimal LowStockThreshold { get; set; } = 5m;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}