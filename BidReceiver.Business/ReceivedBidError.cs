using BidReceiver.Domain;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BidReceiver.Business
{
    public class ReceivedBidError : IConsumer<ReceivedBidMessage>
    {
        private readonly ILogger<ReceivedBidConsumer> _logger;
        private readonly IMediator _mediator;

        public ReceivedBidError(ILogger<ReceivedBidConsumer> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ReceivedBidMessage> context)
        {
            // TODO: OnError
        }
    }
}
