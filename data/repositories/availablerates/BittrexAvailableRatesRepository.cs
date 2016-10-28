using System.Threading.Tasks;
using System.Linq;
using data.repositories.currency;
using MyCryptos.models;
using data.database.models;
using data.storage;
using System.Collections.Generic;
using data.repositories.exchangerate;
using System.Diagnostics;
using System;

namespace data.repositories.availablerates
{
	public class BittrexAvailableRatesRepository : AvailableRatesRepository
	{
		List<ExchangeRate> Elements;

		public BittrexAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
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
				var repository = (await CurrencyStorage.Instance.Repositories()).Find(r => r is BittrexCurrencyRepository);

				var btc = repository.Elements.ToList().Find(c => c.Equals(Currency.BTC));
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
			return (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is BittrexExchangeRateRepository);
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

