using Microsoft.EntityFrameworkCore;
using Serilog;
using BasicInventoryManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set minimum logging level
    .WriteTo.Console() // Log to console
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Log to file with daily rolling
    .CreateLogger();

// Use Serilog for logging
builder.Host.UseSerilog();

try
{
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Set up the database context

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Login}/{id?}"); // Default route

    app.Run(); // Run the application
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed"); // Log any startup failures
}
finally
{
    Log.CloseAndFlush(); // Ensure log is flushed before application exits
}
