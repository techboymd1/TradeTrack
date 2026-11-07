using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Products;

public class CreateModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public Product Product { get; set; } = new();

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
        {
            return Page();
        }

        try
        {
            _context.Products.Add(Product);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product saved successfully.";

            return RedirectToPage("/Products/Index");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Unable to save product {Sku}.",
                Product.Sku);

            ModelState.AddModelError(
                string.Empty,
                "The product could not be saved. Check that the SKU is unique and try again.");

            return Page();
        }
    }
}