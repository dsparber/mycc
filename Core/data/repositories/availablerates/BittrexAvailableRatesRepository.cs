using System.Threading.Tasks;
using System.Linq;
using data.repositories.currency;
using MyCryptos.models;
using data.database.models;
using data.storage;
using System.Collections.Generic;
using data.repositories.exchangerate;

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

		public override Task<bool> Fetch()
		{
			return Task.Factory.StartNew(() =>
			{
				var repository = CurrencyStorage.Instance.RepositoryOfType<BittrexCurrencyRepository>();
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == repository.Id).Select(e => e.Code);

				Elements = CurrencyStorage.Instance.AllElements.Where(e => codes.Contains(e.Code)).Select(e => new ExchangeRate(Currency.BTC, e)).ToList();
				return true;
			});
		}

		public override ExchangeRateRepository ExchangeRateRepository
		{
			get
			{
				return ExchangeRateStorage.Instance.Repositories.OfType<BittrexExchangeRateRepository>().FirstOrDefault();
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

