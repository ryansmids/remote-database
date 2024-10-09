using Microsoft.EntityFrameworkCore;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<BuitenTemperatuur> BuitenTemperatuur { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuitenTemperatuur>(entiteit =>
        {
            entiteit.HasKey(e => e.Id);
            entiteit.Property(e => e.Temperatuur).HasColumnType("decimal(5,2)").IsRequired();

            // Zorg ervoor dat dit correct is
           entiteit.Property(e => e.Tijd)
               .IsRequired()
               .HasColumnType("timestamp with time zone");  // Gebruik "TIMESTAMPTZ"


            entiteit.Property(e => e.Locatie).IsRequired().HasMaxLength(255);
        });
    }

}


public class BuitenTemperatuur
{
    public int Id { get; set; }
    public decimal Temperatuur { get; set; }
    public DateTime Tijd { get; set; }
    public string Locatie { get; set; }
}