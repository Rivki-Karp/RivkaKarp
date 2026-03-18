namespace Backend.Api.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Bid> Bids => Set<Bid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Auction>()
            .Property(a => a.StartingPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Auction>()
            .Property(a => a.CurrentHighestBid)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bid>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Auction>()
            .Property(a => a.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Auction>()
            .HasMany(a => a.Bids)
            .WithOne(b => b.Auction)
            .HasForeignKey(b => b.AuctionId);
    }
}