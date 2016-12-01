using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Currency;
using MyCryptos.Core.Repositories.ExchangeRates;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Repositories.AvailableRates
{
    public class BtceAvailableRatesRepository : AvailableRatesRepository
    {
        List<ExchangeRate> Elements;

        public BtceAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name)
        {
            Elements = new List<ExchangeRate>();
        }

        public override bool IsAvailable(ExchangeRate element)
        {
            return Elements.Contains(element);
        }

        public override Task<bool> Fetch()
        {
            return Task.Factory.StartNew(() =>
            {
                var repository = CurrencyStorage.Instance.RepositoryOfType<BtceCurrencyRepository>();
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == repository.Id).Select(e => e.Code);

                Elements = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e.Code)).Select(e => new ExchangeRate(Models.Currency.BTC, e)).ToList();
                return true;
            });
        }

        public override ExchangeRateRepository ExchangeRateRepository
        {
            get
            {
                return ExchangeRateStorage.Instance.Repositories.Find(r => r is BtceExchangeRateRepository);
            }
        }
        public override ExchangeRate ExchangeRateWithCurrency(Models.Currency currency)
        {
            return Elements.ToList().Find(e => e.Contains(currency));
        }

        public override List<ExchangeRate> ExchangeRatesWithCurrency(Models.Currency currency)
        {
            return Elements.Where(e => e.Contains(currency)).ToList();
        }
    }
}