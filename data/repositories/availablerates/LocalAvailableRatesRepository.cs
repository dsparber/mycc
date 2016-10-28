using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.exchangerate;
using data.storage;
using MyCryptos.models;

namespace data.repositories.availablerates
{
	public class LocalAvailableRatesRepository : AvailableRatesRepository
	{
		IEnumerable<ExchangeRate> Elements;

		public LocalAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name)
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
				Elements = (await ExchangeRateStorage.Instance.GetLocalRepository()).Elements;
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}

		public override Task<bool> FetchFast()
		{
			return Fetch();
		}

		public async override Task<ExchangeRateRepository> ExchangeRateRepository()
		{
			return await ExchangeRateStorage.Instance.GetLocalRepository();
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