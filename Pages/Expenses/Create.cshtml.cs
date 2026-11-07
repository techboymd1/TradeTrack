using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Expenses;

public class CreateModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    private static readonly HashSet<string> Categories = new()
    {
        "Rent",
        "Utilities",
        "Transport",
        "Salaries",
        "Marketing",
        "Internet",
        "Maintenance",
        "Packaging",
        "Other"
    };

    private static readonly HashSet<string> PaymentMethods = new()
    {
        "Cash",
        "Bank Transfer",
        "POS",
        "Card",
        "Other"
    };

    [BindProperty]
    public Expense Expense { get; set; } = new()
    {
        ExpenseDate = DateTime.Now,
        PaymentMethod = "Cash",
        Category = "Other"
    };

    public CreateModel(
        TradeTrackDbContext context,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void OnGet()
    {
        Expense.ExpenseDate = DateTime.Now;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Categories.Contains(Expense.Category))
        {
            ModelState.AddModelError(
                "Expense.Category",
                "Select a valid expense category.");
        }

        if (!PaymentMethods.Contains(Expense.PaymentMethod))
        {
            ModelState.AddModelError(
                "Expense.PaymentMethod",
                "Select a valid payment method.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Expense.Title = Expense.Title.Trim();
        Expense.Category = Expense.Category.Trim();
        Expense.PaymentMethod = Expense.PaymentMethod.Trim();

        Expense.Description = string.IsNullOrWhiteSpace(Expense.Description)
            ? null
            : Expense.Description.Trim();

        Expense.ReferenceNumber = string.IsNullOrWhiteSpace(Expense.ReferenceNumber)
            ? null
            : Expense.ReferenceNumber.Trim();

        Expense.Notes = string.IsNullOrWhiteSpace(Expense.Notes)
            ? null
            : Expense.Notes.Trim();

        try
        {
            _context.Expenses.Add(Expense);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Expense recorded successfully.";

            return RedirectToPage("/Expenses/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Unable to record expense {Title}.",
                Expense.Title);

            ModelState.AddModelError(
                string.Empty,
                "The expense could not be saved. Please check the information and try again.");

            return Page();
        }
    }
}