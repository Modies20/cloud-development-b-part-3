using Microsoft.EntityFrameworkCore;
using ABCRetailApp.Models;

namespace ABCRetailApp.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Azure SQL Database
    /// Manages Customer, Product, and Order entities with proper relationships
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for each entity
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);
                entity.HasIndex(c => c.Email).IsUnique();
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Phone).HasMaxLength(20);
                entity.Property(c => c.Address).IsRequired().HasMaxLength(200);
                entity.Property(c => c.DateCreated).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
                entity.Property(p => p.StockQuantity).IsRequired();
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
                entity.Property(p => p.DateCreated).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.Quantity).IsRequired();
                entity.Property(o => o.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.Notes).HasMaxLength(1000);
                entity.Property(o => o.Status).IsRequired().HasMaxLength(30).HasDefaultValue("New");
                entity.Property(o => o.DateCreated).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                entity.HasOne(o => o.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(o => o.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Create indexes for foreign keys
                entity.HasIndex(o => o.CustomerId);
                entity.HasIndex(o => o.ProductId);
                entity.HasIndex(o => o.DateCreated);
            });

            // Seed some initial data (optional)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "0821234567",
                    Address = "123 Main St, Johannesburg, 2000",
                    DateCreated = DateTime.UtcNow
                },
                new Customer
                {
                    CustomerId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Phone = "0827654321",
                    Address = "456 Oak Ave, Cape Town, 8001",
                    DateCreated = DateTime.UtcNow
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Laptop",
                    Description = "High-performance laptop for professionals",
                    Price = 15999.99m,
                    Category = "Electronics",
                    StockQuantity = 50,
                    DateCreated = DateTime.UtcNow
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Office Chair",
                    Description = "Ergonomic office chair with lumbar support",
                    Price = 2499.99m,
                    Category = "Furniture",
                    StockQuantity = 100,
                    DateCreated = DateTime.UtcNow
                }
            );
        }
    }
}

