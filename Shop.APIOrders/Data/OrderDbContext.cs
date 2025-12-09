using Microsoft.EntityFrameworkCore;
using Shop.APIOrders.Models;

namespace Shop.APIOrders.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Id)
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(o => o.UserId)
                    .HasMaxLength(450)
                    .IsRequired();

                entity.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(o => o.Status)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(o => o.ShippingAddress)
                    .HasMaxLength(500);

                entity.Property(o => o.CreatedAt)
                    .IsRequired();

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(o => o.UserId);
                entity.HasIndex(o => o.CreatedAt);
            });

            // Configuración de OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.OrderId)
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(oi => oi.ProductId)
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(oi => oi.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(oi => oi.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                entity.Ignore(oi => oi.Subtotal);

                entity.HasIndex(oi => oi.ProductId);
            });
        }
    }
}