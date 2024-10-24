using BidReceiver.Domain;
using MediatR;

namespace BidReceiver.Business.RequestHandlers.Requests
{
    public class CheckBid : IRequest<BidState>
    {
        public Guid BidId { get; set; }
    }
}
