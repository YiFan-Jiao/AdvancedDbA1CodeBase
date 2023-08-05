using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            TestDbContext db = new TestDbContext(serviceProvider.GetRequiredService<DbContextOptions<TestDbContext>>());

            db.Database.EnsureDeleted();
            db.Database.Migrate();
            
            // Add brands
            Brand brandOne = new Brand { Name = "Apple" };
            Brand brandTwo = new Brand { Name = "Lenovo" };
            Brand brandThree = new Brand { Name = "Dell" };

            if (!db.Brands.Any())
            {
                db.Add(brandOne);
                db.Add(brandTwo);
                db.Add(brandThree);
                db.SaveChanges();
            }

            // Add laptops
            Laptop laptopOne = new Laptop
            {
                ModelName = "Legion5",
                Price = 1000.00m,
                Condition = LaptopCondition.New,
                QuantityInStock = 10,
                Brand = brandTwo
            };
            Laptop laptopTwo = new Laptop
            {
                ModelName = "Alienware",
                Price = 2000.00m,
                Condition = LaptopCondition.Refurbished,
                QuantityInStock = -10,
                Brand = brandThree
            };
            Laptop laptopThree = new Laptop
            {
                ModelName = "Apple pro",
                Price = 3000.00m,
                Condition = LaptopCondition.Rental,
                QuantityInStock = 20,
                Brand = brandOne
            };

            if (!db.Laptops.Any())
            {
                db.Add(laptopOne);
                db.Add(laptopTwo);
                db.Add(laptopThree);
                db.SaveChanges();
            }
            
            // Add stores
            Store store1 = new Store { StreetName = "Main Street", StreetNumber = "123", Province = "ON" };
            Store store2 = new Store { StreetName = "First Avenue", StreetNumber = "456", Province = "BC" };
            Store store3 = new Store { StreetName = "Market Road", StreetNumber = "789", Province = "AB" };

            if (!db.Stores.Any())
            {
                db.Add(store1);
                db.Add(store2);
                db.Add(store3);
                db.SaveChanges();
            }

            //add StoreLaptops
            StoreLaptop storeLaptop = new StoreLaptop
            {
                StoreId = store1.Id,
                LaptopId = laptopThree.Id
            };
            StoreLaptop storeLaptop2 = new StoreLaptop
            {
                StoreId = store1.Id,
                LaptopId = laptopOne.Id
            };
            StoreLaptop storeLaptop3 = new StoreLaptop
            {
                StoreId = store2.Id,
                LaptopId = laptopOne.Id
            };
            StoreLaptop storeLaptop4 = new StoreLaptop
            {
                StoreId = store3.Id,
                LaptopId = laptopTwo.Id
            };
            db.Add(storeLaptop);
            if (!db.StoreLaptops.Any())
            {
                db.Add(storeLaptop);
                db.Add(storeLaptop2);
                db.Add(storeLaptop3);
                db.Add(storeLaptop4);
                db.SaveChanges();
            }

            await db.SaveChangesAsync();
        }
    }
}
