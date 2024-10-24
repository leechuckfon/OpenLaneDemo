namespace BidReceiver.Domain
{
    public class Lot { 
        public Guid Id { get; set; } 
        public double MinimumBid { get; set; }
        public Guid? CurrentBidId { get; set; }
    }
}
