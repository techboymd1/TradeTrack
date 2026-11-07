using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class SaleItem
{
    public ulong SaleItemId { get; set; }

    public ulong SaleId { get; set; }

    public ulong ProductId { get; set; }

    [Required]
    [StringLength(150)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string Sku { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "999999999.999")]
    public decimal Quantity { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal UnitCost { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal LineTotal { get; set; }

    public Sale Sale { get; set; } = null!;

    public Product Product { get; set; } = null!;
}