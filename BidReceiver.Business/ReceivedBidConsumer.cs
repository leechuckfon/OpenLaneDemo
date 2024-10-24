using BidReceiver.Business.RequestHandlers.Requests;
using BidReceiver.Domain;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BidReceiver.Business
{
    public class ReceivedBidConsumer : IConsumer<ReceivedBidMessage>
    {
        private readonly ILogger<ReceivedBidConsumer> _logger;
        private readonly IMediator _mediator;

        public ReceivedBidConsumer(ILogger<ReceivedBidConsumer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ReceivedBidMessage> context)
        {
            // Bid Received
            _logger.LogInformation(JsonSerializer.Serialize(context.Message));

            var newState = await _mediator.Send(new CheckBid
            {
                BidId = context.Message.BidId
            });

            _logger.LogInformation(Enum.GetName(newState));


        }
    }
}
