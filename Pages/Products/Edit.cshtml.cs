using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Products;

public class EditModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public Product Product { get; set; } = new();

    public EditModel(
        TradeTrackDbContext context,
        ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(ulong id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        Product = product;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(ulong id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = Product.Name;
        existingProduct.Sku = Product.Sku;
        existingProduct.Category = Product.Category;
        existingProduct.Unit = Product.Unit;
        existingProduct.CostPrice = Product.CostPrice;
        existingProduct.SellingPrice = Product.SellingPrice;
        existingProduct.QuantityInStock = Product.QuantityInStock;
        existingProduct.LowStockThreshold = Product.LowStockThreshold;
        existingProduct.IsActive = Product.IsActive;

        try
        {
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Product updated successfully.";

            return RedirectToPage("/Products/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Unable to update product {Sku}.",
                Product.Sku);

            ModelState.AddModelError(
                string.Empty,
                "The product could not be updated. Check that the SKU is unique and try again.");

            return Page();
        }
    }
}