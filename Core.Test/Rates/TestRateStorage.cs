using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Database;
using MyCC.Core.Rates;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Test.Database;
using NUnit.Framework;

namespace MyCC.Core.Test.Rates
{
    [TestFixture]
    public class TestRateStorage
    {
        private static readonly IEnumerable<RateDescriptor> TestDescriptors = new[]
        {
            new RateDescriptor("ETH1", "ETC1"),
            new RateDescriptor("ETH1", "ETC1").Inverse(),
            new RateDescriptor("DASH1", "USD0"),
            new RateDescriptor("CHF0", "CAD0"),
            new RateDescriptor("CHF0", "DOGE1"),
            new RateDescriptor("13371", "CAD0")
        };

        [OneTimeSetUp]
        public async Task Setup()
        {
            DatabaseUtil.SqLiteConnection = new SqLiteConnection();
            await CurrencyStorage.Instance.LoadOnline();
        }

        [Test]
        public async Task DefaultValuesAvailable()
        {
            await RateStorage.FetchAllNeededRates();

            Assert.IsNotNull(RateConstants.BtcUsdDescriptor.GetRate());
            Assert.IsNotNull(RateConstants.BtcUsdDescriptor.GetRate());
            Assert.IsNotNull(new RateDescriptor("EUR0", "USD0").GetRate());
        }

        [Test]
        public async Task DataAvailable()
        {
            await RateStorage.FetchRates(TestDescriptors);

            foreach (var descriptor in TestDescriptors)
            {
                Assert.IsNotNull(descriptor.GetRate(), descriptor.ToString());
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "MyCC.db"));
            }
            catch { /**/ }
        }
    }
}