using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Repositories;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Database;
using MyCryptos.Core.ExchangeRate.Storage;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
    {
        IEnumerable<Currency.Model.Currency> Currencies;

        public CryptonatorAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
        {
            Currencies = new List<Currency.Model.Currency>();
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

        public override bool IsAvailable(Model.ExchangeRate element)
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

        public override Model.ExchangeRate ExchangeRateWithCurrency(Currency.Model.Currency currency)
        {
            if (Currencies.Contains(currency))
            {
                return new Model.ExchangeRate(currency, Currency.Model.Currency.BTC);
            }
            return null;
        }

        public override List<Model.ExchangeRate> ExchangeRatesWithCurrency(Currency.Model.Currency currency)
        {
            if (Currencies.Contains(currency))
            {
                return new List<Model.ExchangeRate> { new Model.ExchangeRate(currency, Currency.Model.Currency.BTC), new Model.ExchangeRate(currency, Currency.Model.Currency.EUR), new Model.ExchangeRate(currency, Currency.Model.Currency.USD) }.Where(e => !e.ReferenceCurrency.Equals(e.SecondaryCurrency)).ToList();
            }
            return new List<Model.ExchangeRate>();
        }
    }
}