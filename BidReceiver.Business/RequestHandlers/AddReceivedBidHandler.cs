using BidReceiver.Business.Commands.Notifications;
using BidReceiver.Domain;
using MediatR;

namespace BidReceiver.Business.RequestHandlers.Requests
{
    public class AddReceivedBidHandler : IRequestHandler<AddReceivedBid, Guid>
    {
        private readonly AuctionDbContext _context;
        private readonly IMediator _mediator;

        public AddReceivedBidHandler(AuctionDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(AddReceivedBid request, CancellationToken cancellationToken)
        {
            _context.Bids.Add(request.Bid);

            await _context.SaveChangesAsync();

            await _mediator.Publish(new AddBidLog
            {
               Bid = request.Bid
            });

            return request.Bid.Id;
        }
    }
}
