using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.ExchangeRate.Database;
using MyCC.Core.ExchangeRate.Storage;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
    {
        private IEnumerable<Currency.Model.Currency> Currencies;

        public CryptonatorAvailableRatesRepository(int id) : base(id)
        {
            Currencies = new List<Currency.Model.Currency>();
        }
        public override int RepositoryTypeId => AvailableRatesRepositoryDbm.DB_TYPE_CRYPTONATOR_REPOSITORY;

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
            return Currencies.Contains(currency) ? new Model.ExchangeRate(currency, Currency.Model.Currency.Btc) : null;
        }

        public override List<Model.ExchangeRate> ExchangeRatesWithCurrency(Currency.Model.Currency currency)
        {
            return Currencies.Contains(currency) ? new List<Model.ExchangeRate> { new Model.ExchangeRate(currency, Currency.Model.Currency.Btc), new Model.ExchangeRate(currency, Currency.Model.Currency.Eur), new Model.ExchangeRate(currency, Currency.Model.Currency.Usd) }.Where(e => !e.ReferenceCurrency.Equals(e.SecondaryCurrency)).ToList() : new List<Model.ExchangeRate>();
        }
    }
}