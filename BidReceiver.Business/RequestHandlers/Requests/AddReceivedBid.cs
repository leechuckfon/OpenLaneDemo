using BidReceiver.Domain;
using MediatR;

namespace BidReceiver.Business.RequestHandlers.Requests
{
    public class AddReceivedBid : IRequest<Guid>
    {
        public Bid Bid { get; set; }
    }
}
