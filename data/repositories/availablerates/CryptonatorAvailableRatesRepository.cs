using System.Threading.Tasks;
using MyCryptos.models;
using data.database.models;
using data.storage;
using data.repositories.currency;
using System.Collections.Generic;
using data.repositories.exchangerate;
using System.Linq;

namespace data.repositories.availablerates
{
	public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
	{
		IEnumerable<Currency> Currencies;

		public CryptonatorAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
		{
			Currencies = new List<Currency>();
		}

		public override Task<bool> Fetch()
		{
			return Task.Factory.StartNew(() =>
			{
				var repository = CurrencyStorage.Instance.RepositoryOfType<BittrexCurrencyRepository>();
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == repository.Id).Select(e => e.Code);

				Currencies = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e.Code)).ToList();
				return true;
			});
		}

		public override bool IsAvailable(ExchangeRate element)
		{
			return Currencies.Contains(element.ReferenceCurrency) && Currencies.Contains(element.SecondaryCurrency);
		}

		public override ExchangeRateRepository ExchangeRateRepository
		{
			get
			{
				return ExchangeRateStorage.Instance.Repositories.Find(r => r is CryptonatorExchangeRateRepository);
			}
		}

		public override ExchangeRate ExchangeRateWithCurrency(Currency currency)
		{
			if (Currencies.Contains(currency))
			{
				return new ExchangeRate(currency, Currencies.ToList().Find(c => c.Equals(Currency.BTC)));
			}
			return null;
		}
	}
}