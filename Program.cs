using ABCRetailApp.Services;
using ABCRetailApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Azure Storage Service (for Blob, Queue, Files)
builder.Services.AddSingleton<IAzureStorageService, AzureStorageService>();

// Register Azure SQL Database with Entity Framework Core
var sqlConnectionString = builder.Configuration.GetConnectionString("AzureSqlDatabase");
if (!string.IsNullOrEmpty(sqlConnectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(sqlConnectionString,
            sqlServerOptions => sqlServerOptions
                .EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
                .CommandTimeout(30)));

    // Register SQL Data Service
    builder.Services.AddScoped<ISqlDataService, SqlDataService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
