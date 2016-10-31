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

		public override Task<bool> Fetch()
		{
			return Task.Factory.StartNew(() =>
			{
				Elements = ExchangeRateStorage.Instance.LocalRepository.Elements;
				return true;
			});
		}

		public override Task<bool> FetchFast()
		{
			return Fetch();
		}

		public override ExchangeRateRepository ExchangeRateRepository
		{
			get
			{
				return ExchangeRateStorage.Instance.LocalRepository;
			}
		}

		public override ExchangeRate ExchangeRateWithCurrency(Currency currency)
		{
			return Elements.ToList().Find(e => e.Contains(currency));
		}

		public override List<ExchangeRate> ExchangeRatesWithCurrency(Currency currency)
		{
			return Elements.Where(e => e.Contains(currency)).ToList();
		}
	}
}