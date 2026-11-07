using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Customers;

public class CreateModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public CreateModel(
        TradeTrackDbContext context,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Customer.FullName = Customer.FullName.Trim();
        Customer.Phone = Clean(Customer.Phone);
        Customer.Email = Clean(Customer.Email);
        Customer.Address = Clean(Customer.Address);
        Customer.Notes = Clean(Customer.Notes);
        Customer.IsActive = true;

        try
        {
            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Customer added successfully.";

            return RedirectToPage("/Customers/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to save customer.");

            ModelState.AddModelError(
                string.Empty,
                "The customer could not be saved. Please try again.");

            return Page();
        }
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}