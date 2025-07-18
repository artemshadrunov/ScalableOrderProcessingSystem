namespace Implementation.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Implementation.Models;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация для Aurora/PostgreSQL
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.CartId).HasColumnType("uuid");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalCents);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
            entity.Property(e => e.ExpiresAt).HasColumnType("timestamp");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Sku);
            entity.Property(e => e.Sku).HasMaxLength(100);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.Reserved);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId);
            entity.Property(e => e.CartId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.Sku });
            entity.Property(e => e.CartId).HasColumnType("uuid");
            entity.Property(e => e.Sku).HasMaxLength(100);
            entity.Property(e => e.Qty);
            entity.Property(e => e.PriceCents);
        });
    }
} 