namespace BidReceiver.Domain
{
    public enum BidState: byte
    {
        None = 0,
        Received = 1,
        Accepted = 2,
        Rejected = 3
    }
}
