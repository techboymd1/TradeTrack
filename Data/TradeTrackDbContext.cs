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
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Products

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.ProductId)
                .HasColumnName("product_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Sku).HasColumnName("sku").HasMaxLength(40).IsRequired();
            entity.HasIndex(p => p.Sku).IsUnique();

            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(p => p.Category).HasColumnName("category").HasMaxLength(80);
            entity.Property(p => p.Unit).HasColumnName("unit").HasMaxLength(20).IsRequired();

            entity.Property(p => p.CostPrice).HasColumnName("cost_price").HasPrecision(12, 2);
            entity.Property(p => p.SellingPrice).HasColumnName("selling_price").HasPrecision(12, 2);
            entity.Property(p => p.QuantityInStock).HasColumnName("quantity_in_stock").HasPrecision(12, 3);
            entity.Property(p => p.LowStockThreshold).HasColumnName("low_stock_threshold").HasPrecision(12, 3);

            entity.Property(p => p.IsActive).HasColumnName("is_active");

            entity.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate();
        });

        // Sales

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sales");
            entity.HasKey(s => s.SaleId);

            entity.Property(s => s.SaleId)
                .HasColumnName("sale_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.InvoiceNumber)
                .HasColumnName("invoice_number")
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(s => s.InvoiceNumber).IsUnique();

            entity.Property(s => s.CustomerId)
                .HasColumnName("customer_id")
                .HasColumnType("bigint unsigned");

            entity.Property(s => s.SaleDate)
                .HasColumnName("sale_date")
                .HasColumnType("datetime")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2);
            entity.Property(s => s.DiscountAmount).HasColumnName("discount_amount").HasPrecision(12, 2);
            entity.Property(s => s.TotalAmount).HasColumnName("total_amount").HasPrecision(12, 2);

            entity.Property(s => s.PaymentMethod)
                .HasColumnName("payment_method")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(s => s.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("Completed");

            entity.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();

            entity.HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Sale items

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("sale_items");
            entity.HasKey(i => i.SaleItemId);

            entity.Property(i => i.SaleItemId)
                .HasColumnName("sale_item_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(i => i.SaleId).HasColumnName("sale_id").HasColumnType("bigint unsigned");
            entity.Property(i => i.ProductId).HasColumnName("product_id").HasColumnType("bigint unsigned");

            entity.Property(i => i.ProductName).HasColumnName("product_name").HasMaxLength(150).IsRequired();
            entity.Property(i => i.Sku).HasColumnName("sku").HasMaxLength(40).IsRequired();

            entity.Property(i => i.Quantity).HasColumnName("quantity").HasPrecision(12, 3);
            entity.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(12, 2);
            entity.Property(i => i.UnitCost).HasColumnName("unit_cost").HasPrecision(12, 2);
            entity.Property(i => i.LineTotal).HasColumnName("line_total").HasPrecision(12, 2);

            entity.HasOne(i => i.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Expenses

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(e => e.ExpenseId);

            entity.Property(e => e.ExpenseId)
                .HasColumnName("expense_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(80).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(12, 2);
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(30).IsRequired();

            entity.Property(e => e.ExpenseDate)
                .HasColumnName("expense_date")
                .HasColumnType("datetime");

            entity.Property(e => e.ReferenceNumber).HasColumnName("reference_number").HasMaxLength(60);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();
        });

        // Customers

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(c => c.CustomerId);

            entity.Property(c => c.CustomerId)
                .HasColumnName("customer_id")
                .HasColumnType("bigint unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(c => c.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
            entity.Property(c => c.Phone).HasColumnName("phone").HasMaxLength(30);
            entity.Property(c => c.Email).HasColumnName("email").HasMaxLength(150);
            entity.Property(c => c.Address).HasColumnName("address").HasMaxLength(300);
            entity.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(500);
            entity.Property(c => c.IsActive).HasColumnName("is_active");

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAdd();

            entity.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(c => c.FullName);
            entity.HasIndex(c => c.Phone);
            entity.HasIndex(c => c.Email);
        });
    }
}