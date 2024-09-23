using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Database Check gestart.");

var serviceCollection = new ServiceCollection();
serviceCollection.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=192.168.0.76;Username=postgres;Password=REAdmin;Database=KlimaatdbDev"));
serviceCollection.AddTransient<WeerDataApi>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var context = serviceProvider.GetRequiredService<AppDbContext>();

// Zorg ervoor dat de database bestaat en de tabellen correct zijn ingesteld
Log.Information($"Controleren of de database '{context.Database.GetDbConnection().Database}' bestaat.");


await context.Database.MigrateAsync();

// Controleer of de tabel buitentemperatuur gegevens bevat
if (!await context.BuitenTemperatuur.AnyAsync())
{
    Log.Information("Tabel 'buitentemperatuur' bevat nog geen gegevens, wordt nu aangemaakt.");
}
else
{
    Log.Information("Tabel 'buitentemperatuur' bevat al gegevens.");
}

// Verondersteld dat je WeerDataApi al is geïmplementeerd
var apiDataTool = serviceProvider.GetRequiredService<WeerDataApi>();

// Start de weerdata API en begin data te verzamelen
var cancellationTokenSource = new CancellationTokenSource();
await apiDataTool.StartAsync(cancellationTokenSource.Token);

Log.Information("Check beëindigd.");