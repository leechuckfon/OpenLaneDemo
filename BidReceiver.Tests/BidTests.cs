using BidReceiver.Domain;

namespace BidReceiver.Tests
{
    public class Tests
    {
        private Bid testBid;

        [SetUp]
        public void Setup()
        {
            testBid = new Bid();
        }

        #region Happy Flow Tests
        [Test]
        public void CanReceiveNewBid()
        {
            testBid.SetBidState(BidState.Received);
        }

        [Test]
        public void CanAcceptReceivedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Accepted);
        }

        [Test]
        public void CanRejectReceivedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Rejected);
        }
        #endregion

        #region Accept Tests
        [Test]
        public void CannotReceiveAcceptedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Accepted);
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Received));
        }

        [Test]
        public void CannotRejectAcceptedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Accepted);
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Rejected));
        }
        #endregion

        #region Reject Tests
        [Test]
        public void CannotAcceptRejectedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Rejected);
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Accepted));
        }

        [Test]
        public void CannotReceiveRejectedBid()
        {
            testBid.SetBidState(BidState.Received);
            testBid.SetBidState(BidState.Rejected);
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Received));
        }
        #endregion

        #region None Tests
        [Test]
        public void CannotRejectNoneBid()
        {
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Rejected));
        }

        [Test]
        public void CannotAcceptNoneBid()
        {
            Assert.Catch(typeof(InvalidOperationException), () => testBid.SetBidState(BidState.Accepted));
        }
        #endregion



        [Test]
        public void UpdateTimeReceived()
        {
            testBid.SetBidState(BidState.Received);
            DateTime defaultDT = default;

            Assert.That(testBid.BidReceivedTime, Is.Not.EqualTo(defaultDT));
        }

        [Test]
        public void TimeWithValueDoesNotUpdate()
        {
            testBid.SetBidState(BidState.Received);

            var setTime = testBid.BidReceivedTime;

            testBid.SetBidState(BidState.Received);

            Assert.That(testBid.BidReceivedTime, Is.EqualTo(setTime));
        }

        [Test]
        public void NoTimeReceivedSetIsDefault()
        {
            DateTime defaultDT = default;

            Assert.That(testBid.BidReceivedTime, Is.EqualTo(defaultDT));
        }
    }
}