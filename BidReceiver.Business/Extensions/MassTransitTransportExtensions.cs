using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BidReceiver.Business.Extensions
{
    public static class MassTransitTransportExtensions
    {
        // So we don't have to write boilerplate multiple times
        public static IServiceCollection AddMassTransitServiceBusTransport(this IServiceCollection services, Action<IBusRegistrationConfigurator> registerConsumersAction = null, Action<IBusRegistrationContext,IServiceBusBusFactoryConfigurator> configAction = null)
        {
            services.AddMassTransit(x =>
            {
                if (registerConsumersAction is not null)
                {
                    registerConsumersAction(x);
                }

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    // INSERT SERVICE BUS STRING
                    cfg.Host("");
                    
                    if (configAction is not null)
                    {
                        configAction(context,cfg);
                    }
                });
            });

            return services;
        }
    }
}
