using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication2.Data;
using Microsoft.AspNetCore.Http.Json;
using WebApplication2.Models;
using Azure.Core;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestContextConnection"));
});

builder.Services.Configure<JsonOptions>(options => {
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    await SeedData.Initialize(serviceProvider);
}

app.MapGet("/laptops/search", (TestDbContext db, decimal? priceAbove, decimal? priceBelow, string? storeNumber, string? province, LaptopCondition? condition, Guid? brandId, string? searchPhrase) =>
{
    var laptopsQuery = db.Laptops
        .Include(l => l.Brand)
        .AsQueryable();

    if (priceAbove.HasValue)
    {
        laptopsQuery = laptopsQuery.Where(l => l.Price > priceAbove.Value);
    }

    if (priceBelow.HasValue)
    {
        laptopsQuery = laptopsQuery.Where(l => l.Price < priceBelow.Value);
    }

    if (!string.IsNullOrEmpty(storeNumber))
    {
        laptopsQuery = laptopsQuery.Where(l => l.StoreLaptops.Any(sl => sl.Store.StreetNumber == storeNumber && sl.Laptop.QuantityInStock > 0));
    }

    if (!string.IsNullOrEmpty(province))
    {
        laptopsQuery = laptopsQuery.Where(l => l.StoreLaptops.Any(sl => sl.Store.Province == province && sl.Laptop.QuantityInStock > 0));
    }

    if (condition.HasValue)
    {
        laptopsQuery = laptopsQuery.Where(l => l.Condition == condition.Value);
    }

    if (brandId.HasValue)
    {
        laptopsQuery = laptopsQuery.Where(l => l.BrandId == brandId.Value);
    }

    if (!string.IsNullOrEmpty(searchPhrase))
    {
        laptopsQuery = laptopsQuery.Where(l => l.ModelName.Contains(searchPhrase));
    }

    return Results.Ok(laptopsQuery.ToHashSet());
});

app.MapGet("/stores/{storeNumber}/inventory", (TestDbContext db, Guid storeNumber) =>
{
    var store = db.Stores
        .Include(s => s.StoreLaptops)
        .ThenInclude(sl => sl.Laptop)
        .ThenInclude(l => l.Brand)
        .FirstOrDefault(s => s.Id == storeNumber);

    if (store == null)
    {
        return Results.NotFound($"Store with store number {storeNumber} not found.");
    }

    var laptopsInInventory = store.StoreLaptops
        .Where(sl => sl.Laptop.QuantityInStock > 0)
        .Select(sl => sl.Laptop)
        .ToList();

    return Results.Ok(laptopsInInventory);
});

app.MapPost("/stores/{storeNumber}/{laptopNumber}/changeQuantity", async (TestDbContext db, Guid storeNumber, Guid laptopNumber, int amount) =>
{
    var store = await db.Stores.FindAsync(storeNumber);
    var laptop = await db.Laptops.FindAsync(laptopNumber);

    if (store == null || laptop == null)
    {
        return Results.NotFound($"Store with ID {storeNumber} or Laptop with ID {laptopNumber} not found.");
    }

    laptop.QuantityInStock = amount;

    db.Laptops.Update(laptop);
    await db.SaveChangesAsync();

    return Results.Ok($"The quantity of the laptop with ID {laptopNumber} in store with ID {storeNumber} has been updated to {amount}.");
});

app.MapGet("/brands/{brandId}/averagePrice", async (TestDbContext db, Guid brandId) =>
{
    var brand = await db.Brands.FindAsync(brandId);

    if (brand == null)
    {
        return Results.NotFound($"Brand with ID {brandId} not found.");
    }

    var laptops = await db.Laptops
        .Where(l => l.BrandId == brandId)
        .ToListAsync();

    var laptopCount = laptops.Count;
    var averagePrice = laptopCount > 0 ? laptops.Average(l => l.Price) : 0;

    var result = new
    {
        LaptopCount = laptopCount,
        AveragePrice = averagePrice
    };

    return Results.Ok(result);
});

app.MapGet("/stores/groupedByProvince", async (TestDbContext db) =>
{
    var groupedStores = await db.Stores
        .Where(s => !string.IsNullOrEmpty(s.Province))
        .GroupBy(s => s.Province)
        .Select(g => new
        {
            Province = g.Key,
            Stores = g.Select(s => new
            {
                s.Id,
                s.StreetName,
                s.StreetNumber
            }).ToList()
        })
        .ToListAsync();

    return Results.Ok(groupedStores);
});

app.Run();