using BidReceiver.Business.Extensions;
using BidReceiver.Domain;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddBusinessMediatR();
        services.AddLocalSQLServer();
        services.AddMassTransitServiceBusTransport();
    })
    .Build();

host.Run();
