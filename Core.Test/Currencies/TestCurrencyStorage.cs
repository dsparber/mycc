using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Database;
using MyCC.Core.Test.Database;
using NUnit.Framework;

namespace MyCC.Core.Test.Currencies
{
    [TestFixture]
    public class TestCurrencyStorage
    {

        [OneTimeSetUp]
        public async Task Setup()
        {
            DatabaseUtil.SqLiteConnection = new SqLiteConnection();
            await CurrencyStorage.Instance.LoadOnline();
        }

        [Test]
        public void DataAvailable()
        {
            Assert.Greater(CurrencyStorage.Instance.Currencies.Count(), 1250);
        }

        [Test]
        public void FiatCurrenciesAvailable()
        {
            Assert.Greater(CurrencyStorage.Instance.Currencies.Count(c => c.IsFiat), 160);
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