using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Products;

public class ProductIndexModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<ProductIndexModel> _logger;

    public List<Product> Products { get; private set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    public string? LoadError { get; private set; }

    public int TotalProducts => Products.Count(product => product.IsActive);

    public int InStockProducts => Products.Count(product =>
        product.IsActive &&
        product.QuantityInStock > product.LowStockThreshold);

    public int LowStockProducts => Products.Count(product =>
        product.IsActive &&
        product.QuantityInStock > 0 &&
        product.QuantityInStock <= product.LowStockThreshold);

    public int OutOfStockProducts => Products.Count(product =>
        product.IsActive &&
        product.QuantityInStock <= 0);

    public ProductIndexModel(
        TradeTrackDbContext context,
        ILogger<ProductIndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Products = await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Name)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to retrieve the product catalogue.");

            LoadError = "Product information could not be loaded. Please check the database connection.";
        }
    }
}