using System.ComponentModel.DataAnnotations;

namespace TradeTrack.Models;

public class Customer
{
    public ulong CustomerId { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}