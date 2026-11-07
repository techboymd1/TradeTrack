using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Reports;

public class IndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public List<Sale> Sales { get; private set; } = new();
    public List<Expense> Expenses { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateTime FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime ToDate { get; set; }

    public decimal TotalRevenue =>
        Sales.Sum(s => s.TotalAmount);

    public decimal CostOfGoodsSold =>
        Sales.SelectMany(s => s.Items)
             .Sum(i => i.Quantity * i.UnitCost);

    public decimal TotalExpenses =>
        Expenses.Sum(e => e.Amount);

    public decimal GrossProfit =>
        TotalRevenue - CostOfGoodsSold;

    public decimal NetProfit =>
        GrossProfit - TotalExpenses;

    public int TransactionCount => Sales.Count;
    public int ExpenseCount => Expenses.Count;

    public string? LoadError { get; private set; }

    public IndexModel(
        TradeTrackDbContext context,
        ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        SetDefaultDates();

        if (!ValidateDateRange())
            return;

        try
        {
            await LoadReportDataAsync(FromDate, ToDate);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to load business reports.");
            LoadError = "Reports could not be loaded. Check the database connection.";
        }
    }

    public async Task<IActionResult> OnGetExportCsvAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        var start = fromDate?.Date
            ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var end = toDate?.Date ?? DateTime.Today;

        if (start > end)
            return BadRequest("The start date cannot be after the end date.");

        try
        {
            await LoadReportDataAsync(start, end);

            var csv = new StringBuilder();

            csv.AppendLine(
                "Type,Date,Reference,Description,Customer,Category,Payment Method,Revenue,Cost of Goods Sold,Expense,Net Profit");

            foreach (var sale in Sales)
            {
                var products = string.Join(
                    "; ",
                    sale.Items.Select(item =>
                        $"{item.ProductName} x {item.Quantity:0.###}"));

                var cost = sale.Items.Sum(item =>
                    item.Quantity * item.UnitCost);

                csv.AppendLine(CsvRow(
                    "Sale",
                    sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    sale.InvoiceNumber,
                    products,
                    sale.Customer?.FullName ?? "Walk-in Customer",
                    "",
                    sale.PaymentMethod,
                    Money(sale.TotalAmount),
                    Money(cost),
                    Money(0),
                    Money(sale.TotalAmount - cost)
                ));
            }

            foreach (var expense in Expenses)
            {
                csv.AppendLine(CsvRow(
                    "Expense",
                    expense.ExpenseDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    expense.ReferenceNumber ?? "",
                    $"{expense.Title} - {expense.Description}",
                    "",
                    expense.Category,
                    expense.PaymentMethod,
                    Money(0),
                    Money(0),
                    Money(expense.Amount),
                    Money(-expense.Amount)
                ));
            }

            var encoding = new UTF8Encoding(true);
            var bytes = encoding.GetPreamble()
                .Concat(encoding.GetBytes(csv.ToString()))
                .ToArray();

            return File(
                bytes,
                "text/csv",
                $"TradeTrack-Report-{start:yyyyMMdd}-{end:yyyyMMdd}.csv");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to export business report.");
            return StatusCode(500, "The report could not be exported.");
        }
    }

    private void SetDefaultDates()
    {
        if (FromDate == default)
        {
            FromDate = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);
        }

        if (ToDate == default)
            ToDate = DateTime.Today;
    }

    private bool ValidateDateRange()
    {
        if (FromDate.Date > ToDate.Date)
        {
            LoadError = "The start date cannot be after the end date.";
            return false;
        }

        return true;
    }

    private async Task LoadReportDataAsync(
        DateTime startDate,
        DateTime endDate)
    {
        var start = startDate.Date;
        var endExclusive = endDate.Date.AddDays(1);

        Sales = await _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .Where(s =>
                s.Status == "Completed" &&
                s.SaleDate >= start &&
                s.SaleDate < endExclusive)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        Expenses = await _context.Expenses
            .AsNoTracking()
            .Where(e =>
                e.ExpenseDate >= start &&
                e.ExpenseDate < endExclusive)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);

    private static string CsvRow(params string?[] values) =>
        string.Join(",", values.Select(EscapeCsv));

    private static string EscapeCsv(string? value)
    {
        value ??= "";

        if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
            value = "'" + value;

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}