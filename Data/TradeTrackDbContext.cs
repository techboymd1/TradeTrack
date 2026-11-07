using Microsoft.EntityFrameworkCore;
using TradeTrack.Models;

namespace TradeTrack.Data;

public class TradeTrackDbContext : DbContext
{
    public TradeTrackDbContext(
        DbContextOptions<TradeTrackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Products table

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(product => product.ProductId);

            entity.Property(product => product.ProductId)
                .HasColumnName("product_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(product => product.Sku)
                .HasColumnName("sku")
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(product => product.Sku)
                .IsUnique();

            entity.Property(product => product.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(product => product.Category)
                .HasColumnName("category")
                .HasMaxLength(80);

            entity.Property(product => product.Unit)
                .HasColumnName("unit")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(product => product.CostPrice)
                .HasColumnName("cost_price")
                .HasPrecision(12, 2);

            entity.Property(product => product.SellingPrice)
                .HasColumnName("selling_price")
                .HasPrecision(12, 2);

            entity.Property(product => product.QuantityInStock)
                .HasColumnName("quantity_in_stock")
                .HasPrecision(12, 3);

            entity.Property(product => product.LowStockThreshold)
                .HasColumnName("low_stock_threshold")
                .HasPrecision(12, 3);

            entity.Property(product => product.IsActive)
                .HasColumnName("is_active");

            entity.Property(product => product.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();

            entity.Property(product => product.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate();
        });


        // Sales table

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sales");

            entity.HasKey(sale => sale.SaleId);

            entity.Property(sale => sale.SaleId)
                .HasColumnName("sale_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(sale => sale.InvoiceNumber)
                .HasColumnName("invoice_number")
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(sale => sale.InvoiceNumber)
                .IsUnique();

            entity.Property(sale => sale.SaleDate)
                .HasColumnName("sale_date")
                .HasColumnType("datetime")
                .ValueGeneratedOnAdd();

            entity.Property(sale => sale.Subtotal)
                .HasColumnName("subtotal")
                .HasPrecision(12, 2);

            entity.Property(sale => sale.DiscountAmount)
                .HasColumnName("discount_amount")
                .HasPrecision(12, 2);

            entity.Property(sale => sale.TotalAmount)
                .HasColumnName("total_amount")
                .HasPrecision(12, 2);

            entity.Property(sale => sale.PaymentMethod)
                .HasColumnName("payment_method")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(sale => sale.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("Completed");

            entity.Property(sale => sale.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();
        });


        // Sale items table

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("sale_items");

            entity.HasKey(item => item.SaleItemId);

            entity.Property(item => item.SaleItemId)
                .HasColumnName("sale_item_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(item => item.SaleId)
                .HasColumnName("sale_id")
                .HasColumnType("bigint unsigned");

            entity.Property(item => item.ProductId)
                .HasColumnName("product_id")
                .HasColumnType("bigint unsigned");

            entity.Property(item => item.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(item => item.Sku)
                .HasColumnName("sku")
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(item => item.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(12, 3);

            entity.Property(item => item.UnitPrice)
                .HasColumnName("unit_price")
                .HasPrecision(12, 2);

            entity.Property(item => item.UnitCost)
                .HasColumnName("unit_cost")
                .HasPrecision(12, 2);

            entity.Property(item => item.LineTotal)
                .HasColumnName("line_total")
                .HasPrecision(12, 2);

            entity.HasOne(item => item.Sale)
                .WithMany(sale => sale.Items)
                .HasForeignKey(item => item.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}