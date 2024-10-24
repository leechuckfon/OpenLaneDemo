using Microsoft.EntityFrameworkCore;

namespace BidReceiver.Domain
{
    public class AuctionDbContext : DbContext
    {
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<BidLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cause for some reason just doing it doesn't work on AddDBContext ????
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=OpenLane;Trusted_Connection=True;TrustServerCertificate=True;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
