using VehicleMvcApp.Services.Interfaces;
using VehicleMvcApp.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services
builder.Services.AddControllersWithViews();

// Register repository based on configuration
var useDatabase = builder.Configuration.GetValue<bool>("DataSource:UseDatabase");

if (useDatabase)
{
    Console.WriteLine("🗄️ Registering DatabaseVehicleRepository");
    builder.Services.AddScoped<IVehicleRepository, DatabaseVehicleRepository>();
}
else
{
    Console.WriteLine("📄 Registering JsonVehicleRepository");
    builder.Services.AddScoped<IVehicleRepository, JsonVehicleRepository>();
}

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Vehicles}/{action=Index}/{id?}");

Console.WriteLine("🚗 Vehicle Management MVC Application starting...");
Console.WriteLine($"📊 Data Source: {(useDatabase ? "Database" : "JSON File")}");
Console.WriteLine($"🌐 Application running...");

app.Run();
