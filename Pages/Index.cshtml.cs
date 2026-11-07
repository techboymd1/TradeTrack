using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;

namespace TradeTrack.Pages;

public class IndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public int ProductsInStock { get; private set; }
    public int LowStockProducts { get; private set; }

    public decimal SalesToday { get; private set; }
    public decimal MonthlyExpenses { get; private set; }
    public decimal NetProfit { get; private set; }

    public bool InventoryLoaded { get; private set; }
    public bool FinancialDataLoaded { get; private set; }

    public string ProductsInStockDisplay =>
        InventoryLoaded ? ProductsInStock.ToString("N0") : "—";

    public string LowStockProductsDisplay =>
        InventoryLoaded ? LowStockProducts.ToString("N0") : "—";

    public string InventoryStatus =>
        !InventoryLoaded ? "Unavailable" :
        LowStockProducts > 0 ? "Attention Required" :
        "Stock Healthy";

    public string SalesTodayDisplay =>
        FinancialDataLoaded ? $"₦{SalesToday:N2}" : "—";

    public string MonthlyExpensesDisplay =>
        FinancialDataLoaded ? $"₦{MonthlyExpenses:N2}" : "—";

    public string NetProfitDisplay =>
        FinancialDataLoaded ? $"₦{NetProfit:N2}" : "—";

    public IndexModel(
        TradeTrackDbContext context,
        ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var monthStart = new DateTime(
            today.Year,
            today.Month,
            1);

        var nextMonth = monthStart.AddMonths(1);

        // Inventory summary

        try
        {
            var activeProducts = _context.Products
                .AsNoTracking()
                .Where(product => product.IsActive);

            ProductsInStock = await activeProducts
                .CountAsync(product => product.QuantityInStock > 0);

            LowStockProducts = await activeProducts
                .CountAsync(product =>
                    product.QuantityInStock > 0 &&
                    product.QuantityInStock <= product.LowStockThreshold);

            InventoryLoaded = true;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to load dashboard inventory.");
        }

        // Financial summary

        try
        {
            SalesToday = await _context.Sales
                .AsNoTracking()
                .Where(sale =>
                    sale.Status == "Completed" &&
                    sale.SaleDate >= today &&
                    sale.SaleDate < tomorrow)
                .SumAsync(sale => (decimal?)sale.TotalAmount) ?? 0m;

            var monthlySales = await _context.Sales
                .AsNoTracking()
                .Include(sale => sale.Items)
                .Where(sale =>
                    sale.Status == "Completed" &&
                    sale.SaleDate >= monthStart &&
                    sale.SaleDate < nextMonth)
                .ToListAsync();

            var monthlyRevenue = monthlySales.Sum(
                sale => sale.TotalAmount);

            var costOfGoodsSold = monthlySales
                .SelectMany(sale => sale.Items)
                .Sum(item => item.Quantity * item.UnitCost);

            MonthlyExpenses = await _context.Expenses
                .AsNoTracking()
                .Where(expense =>
                    expense.ExpenseDate >= monthStart &&
                    expense.ExpenseDate < nextMonth)
                .SumAsync(expense => (decimal?)expense.Amount) ?? 0m;

            NetProfit =
                monthlyRevenue -
                costOfGoodsSold -
                MonthlyExpenses;

            FinancialDataLoaded = true;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to load dashboard financial information.");
        }
    }
}