using System.Threading.Tasks;
using System.Linq;
using data.repositories.currency;
using models;
using data.database.models;
using data.storage;
using System.Collections.Generic;
using data.repositories.exchangerate;
using System;
using System.Diagnostics;

namespace data.repositories.availablerates
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

		public override async Task<bool> Fetch()
		{
			try
			{
				var repository = (await CurrencyStorage.Instance.Repositories()).Find(r => r is BtceCurrencyRepository);

				var btc = repository.Elements.Find(c => c.Equals(Currency.BTC));
				Elements = repository.Elements.Select(e => new ExchangeRate(btc, e)).ToList();
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}

		public async override Task<ExchangeRateRepository> ExchangeRateRepository()
		{
			return (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is BtceExchangeRateRepository);
		}
		public override ExchangeRate ExchangeRateWithCurrency(Currency currency)
		{
			foreach (var e in Elements)
			{
				if (e.Contains(currency))
				{
					return e;
				}
			}
			return null;
		}
	}
}