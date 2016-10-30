using System.Threading.Tasks;
using System.Linq;
using data.repositories.currency;
using MyCryptos.models;
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

		public override Task<bool> Fetch()
		{
			return Task.Factory.StartNew(() =>
			{
				var repository = CurrencyStorage.Instance.RepositoryOfType<BtceCurrencyRepository>();
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == repository.Id).Select(e => e.Code);

				Elements = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e.Code)).Select(e => new ExchangeRate(Currency.BTC, e)).ToList();
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