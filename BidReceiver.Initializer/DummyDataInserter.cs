using BidReceiver.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BidReceiver.Initializer
{
    public class DummyDataInserter : BackgroundService
    {
        private readonly AuctionDbContext _dbContext;
        private readonly ILogger<DummyDataInserter> _logger;

        public DummyDataInserter(AuctionDbContext dbContext, ILogger<DummyDataInserter> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lots = new List<Guid>();

            // Add Random Lots
            _logger.LogInformation($"Adding random lots");
            var r = new Random();
            foreach (var i in Enumerable.Range(0, 5))
            {
                var newLot = Guid.NewGuid();
                lots.Add(newLot);

                _logger.LogInformation($"Adding Lot {newLot}");

                _dbContext.Lots.Add(new Lot
                {
                    Id = newLot, 
                    MinimumBid = r.NextDouble() * 10
                });
            }

            await _dbContext.SaveChangesAsync();

            var factory = new BidFactory();

            _logger.LogInformation($"Adding random bids");
            foreach (var l in lots)
            {
                _logger.LogInformation($"Adding bids for lot {l}");
                var currentLot = _dbContext.Lots.FirstOrDefault(x => x.Id == l);
                var createdBids = factory.CreateBidsForLot(5, l, currentLot.MinimumBid);

                _logger.LogInformation($"{createdBids.Count()} Bids added to Lot {l} with prices [{string.Join(" | ",createdBids.Select(x => x.BidAmount))}]");
                _dbContext.Bids.AddRange(createdBids);
                await _dbContext.SaveChangesAsync();

                currentLot.CurrentBidId = createdBids.OrderByDescending(x => x.BidAmount).First()?.Id;
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Init Done");
        }
    }

    public class BidFactory
    {
        public IEnumerable<Bid> CreateBidsForLot(int amountOfBids, Guid lot, double minimumBid)
        {
            var generatedBids = new List<Bid>();
            var r = new Random();


            foreach (var i in Enumerable.Range(0, amountOfBids))
            {
                var generatedBid = r.NextDouble() * 10;
                while (generatedBid <= minimumBid)
                {
                    generatedBid = r.NextDouble() * 10;
                }

                var newBid = new Bid
                {
                    BidAmount = generatedBid,
                    LotId = lot,
                    State = BidState.Received,
                    UserGuid = Guid.NewGuid(),
                    BidReceivedTime = DateTime.Now
                };

                newBid.SetBidState(BidState.Accepted);

                generatedBids.Add(newBid);
            }

            return generatedBids;
        }
    }
}
