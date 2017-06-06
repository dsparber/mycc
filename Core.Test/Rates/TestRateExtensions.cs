using MyCC.Core.Currencies;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using NUnit.Framework;

namespace MyCC.Core.Test.Rates
{
    [TestFixture]
    public class TestRateExtensions
    {
        private static readonly string EurId = CurrencyConstants.Eur.Id;
        private static readonly string BtcId = CurrencyConstants.Btc.Id;
        private static readonly string UsdId = CurrencyConstants.Usd.Id;

        [Test]
        public void TestCombineRates()
        {
            var referenceRate = new ExchangeRate(new RateDescriptor(BtcId, EurId), 1000);
            var secondaryRate = new ExchangeRate(new RateDescriptor(BtcId, UsdId), 1200);

            var combinedRate = referenceRate.CombineWith(secondaryRate);

            Assert.AreEqual(combinedRate.Descriptor, new RateDescriptor(EurId, UsdId));
            Assert.AreEqual(combinedRate.Rate, 1.2m);
        }
    }
}
