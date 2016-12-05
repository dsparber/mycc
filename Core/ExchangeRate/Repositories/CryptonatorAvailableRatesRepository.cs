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
    public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
    {
        IEnumerable<Models.Currency> Currencies;

        public CryptonatorAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
        {
            Currencies = new List<Models.Currency>();
        }

        public override Task<bool> FetchOnline()
        {
            return Task.Factory.StartNew(() =>
            {
                var repository = CurrencyStorage.Instance.RepositoryOfType<CryptonatorCurrencyRepository>();
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == repository.Id).Select(e => e.Code);

                Currencies = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e?.Code)).ToList();
                return true;
            });
        }

        public override bool IsAvailable(ExchangeRate element)
        {
            var x = Currencies.Contains(element.ReferenceCurrency) && Currencies.Contains(element.SecondaryCurrency);
            return x;
        }

        public override ExchangeRateRepository ExchangeRateRepository
        {
            get
            {
                return ExchangeRateStorage.Instance.Repositories.Find(r => r is CryptonatorExchangeRateRepository);
            }
        }

        public override ExchangeRate ExchangeRateWithCurrency(Models.Currency currency)
        {
            if (Currencies.Contains(currency))
            {
                return new ExchangeRate(currency, Models.Currency.BTC);
            }
            return null;
        }

        public override List<ExchangeRate> ExchangeRatesWithCurrency(Models.Currency currency)
        {
            if (Currencies.Contains(currency))
            {
                return new List<ExchangeRate> { new ExchangeRate(currency, Models.Currency.BTC), new ExchangeRate(currency, Models.Currency.EUR), new ExchangeRate(currency, Models.Currency.USD) }.Where(e => !e.ReferenceCurrency.Equals(e.SecondaryCurrency)).ToList();
            }
            return new List<ExchangeRate>();
        }
    }
}