using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class TestDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreLaptop>()
                .HasKey(sl => new { sl.StoreId, sl.LaptopId });

            modelBuilder.Entity<StoreLaptop>()
                .HasOne(sl => sl.Store)
                .WithMany(s => s.StoreLaptops)
                .HasForeignKey(sl => sl.StoreId);

            modelBuilder.Entity<StoreLaptop>()
                .HasOne(sl => sl.Laptop)
                .WithMany(l => l.StoreLaptops)
                .HasForeignKey(sl => sl.LaptopId);

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

        public DbSet<StoreLaptop> StoreLaptops { get; set; } = null!;
    }
}
