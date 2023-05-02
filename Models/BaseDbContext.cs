using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Bazaar.Flipper.Models;
/// <summary>
/// <see cref="DbContext"/> For flip tracking
/// </summary>
public class BazaarFlipperDbContext : DbContext
{
    public DbSet<Flip> Flips { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="BazaarFlipperDbContext"/>
    /// </summary>
    /// <param name="options"></param>
    public BazaarFlipperDbContext(DbContextOptions<BazaarFlipperDbContext> options)
    : base(options)
    {
    }

    /// <summary>
    /// Configures additional relations and indexes
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Flip>(entity =>
        {
            entity.HasIndex(e => new { e.AuctionId });
        });
    }
}