using BidReceiver.Domain;
using MediatR;

namespace BidReceiver.Business.Commands.Notifications
{
    public class AddBidLog : INotification
    {
        public Bid Bid { get; set; }
    }
}
