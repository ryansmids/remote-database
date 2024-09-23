using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Hier voeg je je connection string toe
        optionsBuilder.UseNpgsql("Host=192.168.0.76;Username=postgres;Password=REAdmin;Database=KlimaatdbDev");

        return new AppDbContext(optionsBuilder.Options);
    }
}