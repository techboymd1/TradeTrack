using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Sales;

public class IndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public List<Sale> Sales { get; private set; } = new();

    public string? LoadError { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public decimal TotalRevenue =>
        Sales.Sum(sale => sale.TotalAmount);

    public int TotalTransactions => Sales.Count;

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
            Sales = await _context.Sales
                .AsNoTracking()
                .Include(sale => sale.Items)
                .OrderByDescending(sale => sale.SaleDate)
                .Take(100)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to load sales history.");

            LoadError = "Sales history could not be loaded. Check the database connection.";
        }
    }
}