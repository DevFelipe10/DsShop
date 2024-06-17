using DsShop.ProductApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DsShop.ProductApi.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    //Fluent API
    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Category table
        mb.Entity<Category>().HasKey(c => c.CategoryId);

        mb.Entity<Category>()
            .Property(c => c.Name)
                .HasMaxLength(100)
                    .IsRequired();

        // Product table
        mb.Entity<Product>()
            .Property(c => c.Name)
                .HasMaxLength(100)
                    .IsRequired();

        mb.Entity<Product>()
            .Property(c => c.Description)
                .HasMaxLength(255)
                    .IsRequired();

        mb.Entity<Product>()
            .Property(c => c.ImageURL)
                .HasMaxLength(255)
                    .IsRequired();

        mb.Entity<Product>()
            .Property(c => c.Price)
                .HasPrecision(12, 2);

        mb.Entity<Category>()
            .HasMany(c => c.Products)
                .WithOne(c => c.Category)
                    .IsRequired()
                        .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Category>().HasData(
            new Category
            {
                CategoryId = 1,
                Name = "Material Escolar"
            },
            new Category
            {
                CategoryId = 2,
                Name = "Acessórios"
            }
        );

        //base.OnModelCreating(mb);
    }
}
