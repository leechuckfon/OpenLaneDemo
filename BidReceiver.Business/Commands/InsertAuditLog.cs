using BidReceiver.Business.Commands.Notifications;
using BidReceiver.Domain;
using MediatR;

namespace BidReceiver.Business.Commands
{
    public class InsertAuditLog : INotificationHandler<AddBidLog>
    {
        private readonly AuctionDbContext _context;

        public InsertAuditLog(AuctionDbContext context)
        {
            _context = context;
        }
        // Add Audit logs for Bids
        public async Task Handle(AddBidLog request, CancellationToken cancellationToken)
        {
            _context.Logs.Add(new BidLog(request.Bid));
            await _context.SaveChangesAsync();
        }
    }
}
