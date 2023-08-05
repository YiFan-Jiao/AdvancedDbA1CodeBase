using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class TestDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Laptop>()
                .HasOne(l => l.Brand)
                .WithMany(b => b.Laptops)
                .HasForeignKey(l => l.BrandId);

            modelBuilder.Entity<Laptop>()
                .Property(l => l.Price)
                .HasColumnType("decimal(10, 2)");
        }
        public TestDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Laptop> Laptops { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
    }
}
