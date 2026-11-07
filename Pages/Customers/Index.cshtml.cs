using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public List<Customer> Customers { get; private set; } = new();

    public string? LoadError { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public int TotalCustomers => Customers.Count;
    public int ActiveCustomers => Customers.Count(c => c.IsActive);
    public int InactiveCustomers => Customers.Count(c => !c.IsActive);

    public IndexModel(
        TradeTrackDbContext context,
        ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to load customers.");
            LoadError = "Customer records could not be loaded.";
        }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(ulong id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
            return NotFound();

        customer.IsActive = !customer.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = customer.IsActive
            ? "Customer activated successfully."
            : "Customer deactivated successfully.";

        return RedirectToPage();
    }
}