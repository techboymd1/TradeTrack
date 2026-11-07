using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Expenses;

public class EditModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<EditModel> _logger;

    private static readonly HashSet<string> Categories = new()
    {
        "Rent", "Utilities", "Transport", "Salaries",
        "Marketing", "Internet", "Maintenance", "Packaging", "Other"
    };

    private static readonly HashSet<string> PaymentMethods = new()
    {
        "Cash", "Bank Transfer", "POS", "Card", "Other"
    };

    [BindProperty]
    public Expense Expense { get; set; } = new();

    public EditModel(
        TradeTrackDbContext context,
        ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(ulong id)
    {
        var expense = await _context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ExpenseId == id);

        if (expense == null)
            return NotFound();

        Expense = expense;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(ulong id)
    {
        if (!Categories.Contains(Expense.Category))
            ModelState.AddModelError(
                "Expense.Category", "Select a valid category.");

        if (!PaymentMethods.Contains(Expense.PaymentMethod))
            ModelState.AddModelError(
                "Expense.PaymentMethod", "Select a valid payment method.");

        if (!ModelState.IsValid)
            return Page();

        var existingExpense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.ExpenseId == id);

        if (existingExpense == null)
            return NotFound();

        existingExpense.Title = Expense.Title.Trim();
        existingExpense.Category = Expense.Category;
        existingExpense.Description =
            string.IsNullOrWhiteSpace(Expense.Description)
                ? null : Expense.Description.Trim();

        existingExpense.Amount = Expense.Amount;
        existingExpense.PaymentMethod = Expense.PaymentMethod;
        existingExpense.ExpenseDate = Expense.ExpenseDate;

        existingExpense.ReferenceNumber =
            string.IsNullOrWhiteSpace(Expense.ReferenceNumber)
                ? null : Expense.ReferenceNumber.Trim();

        existingExpense.Notes =
            string.IsNullOrWhiteSpace(Expense.Notes)
                ? null : Expense.Notes.Trim();

        try
        {
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Expense updated successfully.";

            return RedirectToPage("/Expenses/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Unable to update expense {ExpenseId}.", id);

            ModelState.AddModelError(
                string.Empty,
                "The expense could not be updated. Please try again.");

            return Page();
        }
    }
}