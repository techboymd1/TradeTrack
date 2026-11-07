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

    public bool InventoryLoaded { get; private set; }

    public string ProductsInStockDisplay =>
        InventoryLoaded ? ProductsInStock.ToString("N0") : "—";

    public string LowStockProductsDisplay =>
        InventoryLoaded ? LowStockProducts.ToString("N0") : "—";

    public string InventoryStatus =>
        !InventoryLoaded
            ? "Unavailable"
            : LowStockProducts > 0
                ? "Attention Required"
                : "Stock Healthy";

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
            var activeProducts = _context.Products
                .AsNoTracking()
                .Where(product => product.IsActive);

            ProductsInStock = await activeProducts.CountAsync(
                product => product.QuantityInStock > 0);

            LowStockProducts = await activeProducts.CountAsync(
                product =>
                    product.QuantityInStock > 0 &&
                    product.QuantityInStock <= product.LowStockThreshold);

            InventoryLoaded = true;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to load inventory information for the dashboard.");
        }
    }
}