using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Models;

namespace Shop.APIProducts.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }

        public DbSet<Categories> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.CreatedAt)
                    .IsRequired();

                entity.HasOne(p => p.Categories)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoriesId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(p => p.Name);
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.HasIndex(c => c.Name)
                    .IsUnique();
            });
        }
    }
}