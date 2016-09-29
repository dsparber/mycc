using System.Threading.Tasks;
using models;
using data.database.models;
using data.storage;
using data.repositories.currency;
using System.Collections.Generic;
using System;
using data.repositories.exchangerate;
using System.Diagnostics;

namespace data.repositories.availablerates
{
	public class CryptonatorAvailableRatesRepository : AvailableRatesRepository
	{
		List<Currency> Currencies;

		public CryptonatorAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
		{
			Currencies = new List<Currency>();
		}

		public override async Task<bool> Fetch()
		{
			try
			{
				var repository = (await CurrencyStorage.Instance.Repositories()).Find(r => r is CryptonatorCurrencyRepository);
				Currencies = repository.Elements; return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
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