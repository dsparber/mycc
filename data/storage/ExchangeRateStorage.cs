using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.database.helper;
using data.database.models;
using data.factories;
using data.repositories.exchangerate;
using models;
using models.helper;

namespace data.storage
{
	public class ExchangeRateStorage : AbstractStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate>
	{
		public async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await Task.WhenAll((await Repositories()).Select(x =>
			{
				if (x.Elements.Contains(exchangeRate))
				{
					return x.FetchExchangeRate(exchangeRate);
				}
				return Task.Factory.StartNew(() => { });
			}));
		}

		public async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchExchangeRateFast(exchangeRate)));
		}

		public override AbstractRepositoryDatabase<ExchangeRateRepositoryDBM> GetDatabase()
		{
			return new ExchangeRateRepositoryDatabase();
		}

		protected override async Task OnFirstLaunch()
		{
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY });
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_BTCE_REPOSITORY });
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY });
		}

		protected override ExchangeRateRepository Resolve(ExchangeRateRepositoryDBM obj)
		{
			return ExchangeRateRepositoryFactory.create(obj);
		}

		static ExchangeRateStorage instance { get; set; }

		public static ExchangeRateStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExchangeRateStorage();
				}
				return instance;
			}
		}

		// Helper
		public async Task<ExchangeRate> GetRate(Currency referenceCurrency, Currency secondaryCurrency, bool fast)
		{
			if (referenceCurrency == null || secondaryCurrency == null)
			{
				return null;
			}

			ExchangeRate rate = await GetDirectRate(referenceCurrency, secondaryCurrency, fast);

			if (rate != null)
			{
				return rate;
			}

			// Indirect match (one intermediate currency)
			var referenceCurrencyRates = new List<ExchangeRate>();
			var secondaryCurrencyRates = new List<ExchangeRate>();

			foreach (ExchangeRate exchangeRate in await AllElements())
			{
				if (exchangeRate.Contains(referenceCurrency))
				{
					referenceCurrencyRates.Add(exchangeRate);
				}
				if (exchangeRate.Contains(secondaryCurrency))
				{
					secondaryCurrencyRates.Add(exchangeRate);
				}
			}

			foreach (ExchangeRate r1 in referenceCurrencyRates)
			{
				foreach (ExchangeRate r2 in secondaryCurrencyRates)
				{
					if (ExchangeRateHelper.OneMatch(r1, r2))
					{
						if (!fast)
						{
							await FetchExchangeRate(r1);
							await FetchExchangeRate(r2);
						}
						return ExchangeRateHelper.GetCombinedRate(r1, r2);
					}
				}
			}
			return null;
		}

		public async Task<ExchangeRate> GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency, bool fast)
		{
			if (referenceCurrency.Equals(secondaryCurrency))
			{
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
			}

			var allElements = await AllElements();

			foreach (ExchangeRate exchangeRate in allElements)
			{
				if (exchangeRate.Equals(new ExchangeRate(referenceCurrency, secondaryCurrency)))
				{
					if (!fast)
					{
						await FetchExchangeRate(exchangeRate);
					}
					return exchangeRate;
				}
				if (exchangeRate.Equals(new ExchangeRate(secondaryCurrency, referenceCurrency)))
				{
					if (!fast)
					{
						await FetchExchangeRate(exchangeRate);
					}
					return exchangeRate.GetInverse();
				}
			}
			return null;
		}


	}
}

