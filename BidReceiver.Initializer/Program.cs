using BidReceiver.Domain;
using BidReceiver.Initializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostBuilder builder = new HostBuilder();

await builder.ConfigureServices(services =>
{
    services.AddLogging(x => x.AddSeq());

    services.AddLocalSQLServer();

    services.AddHostedService<DummyDataInserter>();

}).RunConsoleAsync();