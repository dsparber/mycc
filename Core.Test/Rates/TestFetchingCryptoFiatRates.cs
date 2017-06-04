using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Database;
using MyCC.Core.Rates;
using MyCC.Core.Test.Database;
using NUnit.Framework;

namespace MyCC.Core.Test.Rates
{
    [TestFixture]
    public class TestFetchingCryptoFiatRates
    {
        [SetUp]
        public async Task Setup()
        {
            DatabaseUtil.SqLiteConnection = new SqLiteConnection();
            await CurrencyStorage.Instance.LoadOnline();
            await RateStorage.FetchAllFiatToCryptoRates();
        }

        [Test]
        public void FiatCurrenciesAvailable()
        {
            var allRates = RateStorage.AllCryptoToFiateRates.ToList();
            Assert.AreEqual(16, allRates.Count);
        }

        [Test]
        public void PlausibleValues()
        {
            var allRates = RateStorage.AllCryptoToFiateRates.ToList();
            Assert.Greater(allRates.Min(rate => rate.Rate), 500);
            Assert.Less(allRates.Max(rate => rate.Rate), 5000);
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