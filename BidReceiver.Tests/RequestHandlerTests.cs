using BidReceiver.Business.Extensions;
using BidReceiver.Business.RequestHandlers.Requests;
using BidReceiver.Domain;
using BidReceiver.Initializer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BidReceiver.Tests
{
    public class RequestHandlerTests
    {
        private Mock<DbSet<BidLog>> _mockedLogs;
        private Mock<DbSet<Bid>> _mockedBids;
        private Mock<DbSet<Lot>> _mockedLots;
        private Mock<AuctionDbContext> _mockedContext;
        private List<Bid> _allBids;
        private List<Lot> _lots;

        private IServiceProvider BuildTestProvider()
        {

            var services = new ServiceCollection();

            services.AddBusinessMediatR();

            _lots = new List<Lot>();

            var r = new Random();
            foreach (var i in Enumerable.Range(0, 5))
            {
                var newLot = Guid.NewGuid();

                _lots.Add(new Lot
                {
                    Id = newLot,
                    MinimumBid = r.NextDouble() * 10
                });
            }

            var factory = new BidFactory();

            _allBids = new List<Bid>();

            foreach (var lot in _lots)
            {
                var bids = factory.CreateBidsForLot(5,lot.Id,lot.MinimumBid);

                foreach (var bid in bids)
                {
                    bid.GenerateNewId();
                }

                lot.CurrentBidId = bids.OrderByDescending(x => x.BidAmount).FirstOrDefault().Id;

                _allBids.AddRange(bids);
            }

            var bidData = _allBids.AsQueryable();
            var lotData = _lots.AsQueryable();

            _mockedLogs = new Mock<DbSet<BidLog>>();
            _mockedBids = new Mock<DbSet<Bid>>();

            _mockedLots = new Mock<DbSet<Lot>>();

            SetBidData(bidData);
            SetLotData(lotData);

            _mockedContext = new Mock<AuctionDbContext>();

            _mockedContext.Setup(x => x.Logs).Returns(_mockedLogs.Object);

            _mockedContext.Setup(x => x.Bids).Returns(_mockedBids.Object);

            _mockedContext.Setup(x => x.Lots).Returns(_mockedLots.Object);

            //var mockedLogger = new Mock<ILogger>();

            services.AddTransient<AuctionDbContext>((x) => _mockedContext.Object);
            services.AddLogging();
            //services.AddTransient<ILogger>(x => mockedLogger.Object);
            return services.BuildServiceProvider();
        }

        private void SetBidData(IQueryable<Bid> bidData)
        {
            _mockedBids.As<IQueryable<Bid>>().Setup(m => m.Provider).Returns(bidData.Provider);
            _mockedBids.As<IQueryable<Bid>>().Setup(m => m.Expression).Returns(bidData.Expression);
            _mockedBids.As<IQueryable<Bid>>().Setup(m => m.ElementType).Returns(bidData.ElementType);
            _mockedBids.As<IQueryable<Bid>>().Setup(m => m.GetEnumerator()).Returns(bidData.GetEnumerator());
        }

        private void SetLotData(IQueryable<Lot> lotData)
        {
            _mockedLots.As<IQueryable<Lot>>().Setup(m => m.Provider).Returns(lotData.Provider);
            _mockedLots.As<IQueryable<Lot>>().Setup(m => m.Expression).Returns(lotData.Expression);
            _mockedLots.As<IQueryable<Lot>>().Setup(m => m.ElementType).Returns(lotData.ElementType);
            _mockedLots.As<IQueryable<Lot>>().Setup(m => m.GetEnumerator()).Returns(lotData.GetEnumerator());
        }

        [Test]
        public async Task CheckAcceptedBid()
        {
            var mediator = BuildTestProvider().GetRequiredService<IMediator>();

            var bid = new Bid { BidAmount = 999, LotId = _lots.First().Id,UserGuid = Guid.NewGuid() };

            bid.GenerateNewId();

            bid.SetBidState(BidState.Received);

            _allBids.Add(bid);

            // Add Received Bid
            SetBidData(_allBids.AsQueryable());

            var newBidState = await mediator.Send(new CheckBid
            {
                BidId = bid.Id
            });
            Assert.That(newBidState, Is.EqualTo(BidState.Accepted));
        }

        [Test]
        public async Task CheckRejectedBid()
        {
            var mediator = BuildTestProvider().GetRequiredService<IMediator>();


            var bid = new Bid { BidAmount = 1, LotId = _lots.First().Id, UserGuid = Guid.NewGuid() };

            bid.GenerateNewId();

            bid.SetBidState(BidState.Received);

            _allBids.Add(bid);

            // Add Received Bid
            SetBidData(_allBids.AsQueryable());
            var newBidState = await mediator.Send(new CheckBid
            {
                BidId = bid.Id
            });

            Assert.That(newBidState, Is.EqualTo(BidState.Rejected));
        }

        [Test]
        public async Task CheckAcceptedBidCannotBeRejected()
        {
            var mediator = BuildTestProvider().GetRequiredService<IMediator>();

            
            Assert.CatchAsync(typeof(InvalidOperationException), async () => await mediator.Send(new CheckBid
            {
                BidId = _allBids.First().Id
            }));

        }

        [Test]
        public async Task CheckRejectedBidCannotBeAccepted()
        {
            var mediator = BuildTestProvider().GetRequiredService<IMediator>();

            var bid = new Bid { BidAmount = 999, LotId = _lots.First().Id, UserGuid = Guid.NewGuid() };

            bid.GenerateNewId();

            bid.SetBidState(BidState.Received);

            _allBids.Add(bid);

            bid.SetBidState(BidState.Rejected);

            // Add Received Bid
            SetBidData(_allBids.AsQueryable());

            Assert.CatchAsync(typeof(InvalidOperationException), async () => await mediator.Send(new CheckBid
            {
                BidId = bid.Id
            }));

        }

        [Test]
        public async Task AddReceivedBid()
        {
            var mediator = BuildTestProvider().GetRequiredService<IMediator>();

            await mediator.Send(new AddReceivedBid
            {
                Bid = new Bid()
            });

            _mockedBids.Verify(bids => bids.Add(It.IsAny<Bid>()), Times.Once());
            _mockedLogs.Verify(logs => logs.Add(It.IsAny<BidLog>()), Times.Once());
            _mockedContext.Verify(bids => bids.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
