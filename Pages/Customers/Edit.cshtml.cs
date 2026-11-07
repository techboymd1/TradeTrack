using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Customers;

public class EditModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public EditModel(
        TradeTrackDbContext context,
        ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(ulong id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
            return NotFound();

        Customer = customer;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(ulong id)
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (existing == null)
            return NotFound();

        existing.FullName = Customer.FullName.Trim();
        existing.Phone = Clean(Customer.Phone);
        existing.Email = Clean(Customer.Email);
        existing.Address = Clean(Customer.Address);
        existing.Notes = Clean(Customer.Notes);
        existing.IsActive = Customer.IsActive;

        try
        {
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Customer updated successfully.";

            return RedirectToPage("/Customers/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Unable to update customer {CustomerId}.", id);

            ModelState.AddModelError(
                string.Empty,
                "The customer could not be updated.");

            return Page();
        }
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}