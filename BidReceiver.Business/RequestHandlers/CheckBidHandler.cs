using BidReceiver.Business.Commands.Notifications;
using BidReceiver.Business.RequestHandlers.Requests;
using BidReceiver.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BidReceiver.Business.Commands
{
    public class CheckBidHandler : IRequestHandler<CheckBid, BidState>
    {
        private readonly AuctionDbContext _context;
        private readonly ILogger<CheckBidHandler> _logger;
        private readonly IMediator _mediator;

        public CheckBidHandler(AuctionDbContext context, ILogger<CheckBidHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<BidState> Handle(CheckBid request, CancellationToken cancellationToken)
        {
            var contextBid = _context.Bids.FirstOrDefault(x => x.Id == request.BidId);

            // What if later bid arrives first and earlier bid is delayed?
            while (_context.Bids.Any(bid => bid.BidReceivedTime < contextBid.BidReceivedTime && bid.State == BidState.Received && bid.LotId == contextBid.LotId))
            {
                _logger.LogInformation($"BID {request.BidId} DELAYED: earlier bid exists");
                await Task.Delay(100);
            }

            // Search Lot
            var currentLot = _context.Lots.FirstOrDefault(lot => lot.Id == contextBid.LotId);

            var currentHighestBid = _context.Bids.FirstOrDefault(bid => bid.Id == currentLot.CurrentBidId);

            // Check Lot Price vs Bid
            if (currentHighestBid.BidReceivedTime < contextBid.BidReceivedTime && contextBid.BidAmount > currentHighestBid.BidAmount)
            {

                currentLot.CurrentBidId = contextBid.Id;

                // Reject or Accept Bid
                contextBid.SetBidState(BidState.Accepted);
                _logger.LogInformation($"BID {request.BidId} ACCEPTED: Bid Time {currentHighestBid.BidReceivedTime < contextBid.BidReceivedTime} | Bid Amount {currentHighestBid.BidAmount > currentHighestBid.BidAmount}");
            } else
            {
                contextBid.SetBidState(BidState.Rejected);
                _logger.LogInformation($"BID {request.BidId} REJECTED: Bid Time {currentHighestBid.BidReceivedTime < contextBid.BidReceivedTime} | Bid Amount {currentHighestBid.BidAmount > currentHighestBid.BidAmount}");
            }

            await _context.SaveChangesAsync();


            // Publish New Log
            await _mediator.Publish(new AddBidLog
            {
                Bid = contextBid
            });

            return contextBid.State;
        }
    }
}
