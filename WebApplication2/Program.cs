using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication2.Data;
using Microsoft.AspNetCore.Http.Json;


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

app.Run();