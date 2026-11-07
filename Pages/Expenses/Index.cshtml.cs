using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Expenses;

public class IndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public List<Expense> Expenses { get; private set; } = new();

    public int TotalExpenseRecords { get; private set; }

    public decimal TotalRecordedExpenses { get; private set; }

    public decimal MonthlyExpenses { get; private set; }

    public string? LoadError { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

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
            var expensesQuery = _context.Expenses.AsNoTracking();

            TotalExpenseRecords = await expensesQuery.CountAsync();

            TotalRecordedExpenses =
                await expensesQuery.SumAsync(expense => (decimal?)expense.Amount)
                ?? 0m;

            var monthStart = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            var nextMonth = monthStart.AddMonths(1);

            MonthlyExpenses = await expensesQuery
                .Where(expense =>
                    expense.ExpenseDate >= monthStart &&
                    expense.ExpenseDate < nextMonth)
                .SumAsync(expense => (decimal?)expense.Amount) ?? 0m;

            Expenses = await expensesQuery
                .OrderByDescending(expense => expense.ExpenseDate)
                .ThenByDescending(expense => expense.ExpenseId)
                .Take(100)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to load expense records.");

            LoadError =
                "Expense records could not be loaded. Check the database connection.";
        }
    }
}