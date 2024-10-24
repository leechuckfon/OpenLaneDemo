using BidReceiver.Business;
using BidReceiver.Business.Extensions;
using BidReceiver.Domain;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostBuilder builder = new HostBuilder();

await builder.ConfigureServices(services =>
{
    services.AddLogging(x => x.AddSeq());
    services.AddLocalSQLServer();

    services.AddBusinessMediatR();
    services.AddMassTransitServiceBusTransport(consumeReg =>
    {
        consumeReg.AddConsumer<ReceivedBidConsumer>();
        consumeReg.AddConsumer<ReceivedBidError>();
    },
    (context, cfg) =>
    {
        // Received Bids
        cfg.ReceiveEndpoint("received_bids", x =>
        {
            x.ConfigureConsumer<ReceivedBidConsumer>(context);
        });

        // On Error
        cfg.ReceiveEndpoint("received_bids_error", x =>
        {
            x.ConfigureConsumer<ReceivedBidError>(context);
        });
    });
}).RunConsoleAsync();