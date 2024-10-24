using System.Text.Json;

namespace BidReceiver.Domain
{
    public class BidLog
    {
        public BidLog()
        {
        }

        public BidLog(Bid bid)
        {
            BidData = JsonSerializer.Serialize(bid);
            BidState = bid.State;
            CreationDate = DateTime.Now;
            BidAmount = bid.BidAmount;
            UserGuid = bid.UserGuid;
        }

        public Guid Id { get; set; }
        public string BidData { get; init; }
        public BidState BidState { get; init; }
        public DateTime CreationDate { get; init; }
        public double BidAmount { get; init; }
        public Guid UserGuid { get; init; }
    }
}
