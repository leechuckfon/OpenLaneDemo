namespace BidReceiver.Domain
{
    public class Bid
    {
        public Guid Id { get; private set; }

        public BidState State
        {
            get
            {
                return _innerState;
            }
            set
            {
                _innerState = value;
            }
        }
        private BidState _innerState { get; set; }

        public DateTime BidReceivedTime
        {
            get
            {
                return _innerBidReceivedTime.GetValueOrDefault();
            }
            set
            {
                _innerBidReceivedTime = value;
            }
        }

        private DateTime? _innerBidReceivedTime { get; set; }

        public double BidAmount { get; set; }
        public Guid UserGuid { get; set; }
        public Guid LotId { get; set; }
        public virtual Lot Lot { get; set; }

        public void SetBidState(BidState newState)
        {
            switch (newState)
            {
                case (BidState.Received):
                    if (State == BidState.Accepted || State == BidState.Rejected) 
                        throw new InvalidOperationException($"Cannot receive bid {Id} because it is in state {Enum.GetName(State)}");
                    _innerState = newState;
                    if (!_innerBidReceivedTime.HasValue)
                    {
                        _innerBidReceivedTime = DateTime.Now;
                    }
                    break;
                case (BidState.Accepted): if (State == BidState.Rejected || State == BidState.None) throw new InvalidOperationException($"Cannot accept bid {Id} because it is in state {Enum.GetName(State)}"); _innerState = newState; break;
                case (BidState.Rejected): if (State == BidState.Accepted || State == BidState.None) throw new InvalidOperationException($"Cannot reject bid {Id} because it is in state {Enum.GetName(State)}"); _innerState = newState; break;
                default: _innerState = newState; break;
            }
        }

        public void GenerateNewId()
        {
            Id = Guid.NewGuid();
        }
    }
}
