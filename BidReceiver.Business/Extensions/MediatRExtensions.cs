using BidReceiver.Business.RequestHandlers.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace BidReceiver.Business.Extensions
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddBusinessMediatR(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(AddReceivedBid).Assembly));

            return services;
        }
    }
}
