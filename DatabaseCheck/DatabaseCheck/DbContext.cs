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
        modelBuilder.Entity<BuitenTemperatuur>(entity =>
        {
            entity.ToTable("BuitenTemperatuur");  // Zorg ervoor dat de tabel "BuitenTemperatuur" heet, enkelvoud.
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Temperatuur).HasColumnType("decimal(5,2)").IsRequired();
            entity.Property(e => e.Tijd).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Locatie).IsRequired().HasMaxLength(255);
        });
    }

}


public class BuitenTemperatuur
{
    public int Id { get; set; }
    public decimal Temperatuur { get; set; }
    public string Tijd { get; set; }
    public string Locatie { get; set; }
}