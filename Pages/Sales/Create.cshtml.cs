using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;
using TradeTrack.Models;

namespace TradeTrack.Pages.Sales;

public class CreateModel : PageModel
{
    private readonly TradeTrackDbContext _context;
    private readonly ILogger<CreateModel> _logger;

    private static readonly string[] AllowedPaymentMethods =
    {
        "Cash",
        "Bank Transfer",
        "POS",
        "Card",
        "Other"
    };

    [BindProperty]
    public List<SaleLineInput> Lines { get; set; } = new();

    [BindProperty]
    public ulong? CustomerId { get; set; }

    [BindProperty]
    public decimal DiscountAmount { get; set; }

    [BindProperty]
    public string PaymentMethod { get; set; } = "Cash";

    public List<Product> AvailableProducts { get; private set; } = new();

    public List<Customer> AvailableCustomers { get; private set; } = new();

    public CreateModel(
        TradeTrackDbContext context,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        await LoadFormOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!AllowedPaymentMethods.Contains(
                PaymentMethod,
                StringComparer.Ordinal))
        {
            ModelState.AddModelError(
                nameof(PaymentMethod),
                "Select a valid payment method.");
        }

        if (CustomerId.HasValue)
        {
            var customerExists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(customer =>
                    customer.CustomerId == CustomerId.Value &&
                    customer.IsActive);

            if (!customerExists)
            {
                ModelState.AddModelError(
                    nameof(CustomerId),
                    "The selected customer is unavailable.");
            }
        }

        if (Lines == null || Lines.Count == 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "Add at least one product to the sale.");
        }
        else
        {
            if (Lines.Count > 50)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A sale cannot contain more than 50 product entries.");
            }

            if (Lines.Any(line =>
    line.ProductId == 0 ||
    line.Quantity < 1 ||
    line.Quantity != decimal.Truncate(line.Quantity) ||
    line.Quantity > 999999999.999m))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Select valid products and enter quantities greater than zero.");
            }
        }

        if (DiscountAmount < 0)
        {
            ModelState.AddModelError(
                nameof(DiscountAmount),
                "Discount cannot be negative.");
        }

        if (!ModelState.IsValid)
        {
            await LoadFormOptionsAsync();
            return Page();
        }

        var groupedLines = (Lines ?? new List<SaleLineInput>())
            .GroupBy(line => line.ProductId)
            .Select(group => new SaleLineInput
            {
                ProductId = group.Key,
                Quantity = group.Sum(line => line.Quantity)
            })
            .ToList();

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var saleItems = new List<SaleItem>();
            decimal subtotal = 0;

            foreach (var line in groupedLines)
            {
                var product = await _context.Products
                    .FromSqlInterpolated(
                        $"""
                        SELECT *
                        FROM products
                        WHERE product_id = {line.ProductId}
                        FOR UPDATE
                        """)
                    .SingleOrDefaultAsync();

                if (product == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "One of the selected products could not be found.");

                    await LoadFormOptionsAsync();
                    return Page();
                }

                if (!product.IsActive)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"{product.Name} is inactive and cannot be sold.");

                    await LoadFormOptionsAsync();
                    return Page();
                }

                if (product.QuantityInStock < line.Quantity)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Insufficient stock for {product.Name}. Available: {product.QuantityInStock:0.###}.");

                    await LoadFormOptionsAsync();
                    return Page();
                }

                var lineTotal = decimal.Round(
                    product.SellingPrice * line.Quantity,
                    2,
                    MidpointRounding.AwayFromZero);

                subtotal += lineTotal;

                saleItems.Add(new SaleItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Sku = product.Sku,
                    Quantity = line.Quantity,
                    UnitPrice = product.SellingPrice,
                    UnitCost = product.CostPrice,
                    LineTotal = lineTotal
                });

                product.QuantityInStock -= line.Quantity;
            }

            subtotal = decimal.Round(
                subtotal,
                2,
                MidpointRounding.AwayFromZero);

            if (DiscountAmount > subtotal)
            {
                ModelState.AddModelError(
                    nameof(DiscountAmount),
                    "Discount cannot exceed the sale subtotal.");

                await LoadFormOptionsAsync();
                return Page();
            }

            var totalAmount = decimal.Round(
                subtotal - DiscountAmount,
                2,
                MidpointRounding.AwayFromZero);

            var now = DateTime.Now;

            var invoiceNumber =
                $"TT-{now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            var sale = new Sale
            {
                CustomerId = CustomerId,
                InvoiceNumber = invoiceNumber,
                SaleDate = now,
                Subtotal = subtotal,
                DiscountAmount = DiscountAmount,
                TotalAmount = totalAmount,
                PaymentMethod = PaymentMethod,
                Status = "Completed",
                Items = saleItems
            };

            _context.Sales.Add(sale);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                $"Sale {invoiceNumber} was recorded successfully.";

            return RedirectToPage("/Sales/Index");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unable to complete the sale.");

            ModelState.AddModelError(
                string.Empty,
                "The sale could not be completed. Please try again.");

            await LoadFormOptionsAsync();
            return Page();
        }
    }

    private async Task LoadFormOptionsAsync()
    {
        AvailableProducts = await _context.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive &&
                product.QuantityInStock > 0)
            .OrderBy(product => product.Name)
            .ToListAsync();

        AvailableCustomers = await _context.Customers
            .AsNoTracking()
            .Where(customer => customer.IsActive)
            .OrderBy(customer => customer.FullName)
            .ToListAsync();
    }
}

public class SaleLineInput
{
    public ulong ProductId { get; set; }

    public decimal Quantity { get; set; }
}