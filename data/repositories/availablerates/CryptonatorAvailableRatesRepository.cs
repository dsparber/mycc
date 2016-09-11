using System.Threading.Tasks;
using models;
using data.database.models;
using data.storage;
using data.repositories.currency;
using System.Collections.Generic;
using System;
using data.repositories.exchangerate;

namespace data.repositories.availablerates
{
	public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
	{
		List<Currency> Currencies;

		public CryptonatorAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
		{
			Currencies = new List<Currency>();
		}

		public override async Task Fetch()
		{
			var repository = (await CurrencyStorage.Instance.Repositories()).Find(r => r is CryptonatorCurrencyRepository);

			Currencies = repository.Elements;
		}

		public override bool IsAvailable(ExchangeRate element)
		{
			return Currencies.Contains(element.ReferenceCurrency) && Currencies.Contains(element.SecondaryCurrency);
		}

		public async override Task<ExchangeRateRepository> ExchangeRateRepository()
		{
			return (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is CryptonatorExchangeRateRepository);
		}

		public override ExchangeRate ExchangeRateWithCurrency(Currency currency)
		{
			if (Currencies.Contains(currency))
			{
				return new ExchangeRate(currency, Currencies.Find(c => c.Equals(Currency.BTC)));
			}
			return null;
		}
	}
}