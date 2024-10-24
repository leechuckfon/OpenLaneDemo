using Microsoft.Extensions.DependencyInjection;

namespace BidReceiver.Domain
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddLocalSQLServer(this IServiceCollection services)
        {
            services.AddDbContext<AuctionDbContext>();

            return services;
        }
    }
}
