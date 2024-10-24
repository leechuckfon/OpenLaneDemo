using BidReceiver.Business.RequestHandlers.Requests;
using BidReceiver.Domain;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BidReceiver
{
    public class BidReceived
    {
        private readonly ILogger<BidReceived> _logger;
        private readonly ISendEndpointProvider _sendProvider;
        private readonly IMediator _mediator;

        public BidReceived(ILogger<BidReceived> logger, ISendEndpointProvider sendProvider, IMediator mediator)
        {
            _logger = logger;
            _sendProvider = sendProvider;
            _mediator = mediator;
        }

        [Function("ReceiveBid")]
        public async Task ReceiveBid([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, [FromBody] Bid receivedBid)
        {
            receivedBid.SetBidState(BidState.Received);
            try
            {   
                // Add received bid to database
                var bidId = await _mediator.Send(new AddReceivedBid
                {
                    Bid = receivedBid
                });
                // Get Endpoint to send to and send message
                var endpoint = await _sendProvider.GetSendEndpoint(new Uri("queue:received_bids"));
                await endpoint.Send(new ReceivedBidMessage
                {
                    BidId = bidId
                });
            } catch (Exception e)
            {
                _logger.LogError($"[ERROR] Was not able to queue or save received bid [{JsonSerializer.Serialize(receivedBid)}]: {e.Message}");
            }
        }
    }
}
