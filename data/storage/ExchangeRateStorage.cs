using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.database.helper;
using data.database.models;
using data.factories;
using data.repositories.exchangerate;
using enums;
using models;
using models.helper;

namespace data.storage
{
	public class ExchangeRateStorage : AbstractDatabaseStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate>
	{
		public override AbstractRepositoryDatabase<ExchangeRateRepositoryDBM> GetDatabase()
		{
			return new ExchangeRateRepositoryDatabase();
		}

		protected override async Task OnFirstLaunch()
		{
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY });
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_BTCE_REPOSITORY });
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY });
			await GetDatabase().AddRepository(new ExchangeRateRepositoryDBM { Type = ExchangeRateRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY });
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

		public async Task FetchNew()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchNew()));
		}

		// Helper
		public async Task<ExchangeRate> GetRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
		{
			if (referenceCurrency == null || secondaryCurrency == null)
			{
				return null;
			}

			ExchangeRate rate = await GetDirectRate(referenceCurrency, secondaryCurrency, speed);

			if (rate != null)
			{
				return rate;
			}

			// Indirect match (one intermediate currency)
			var referenceCurrencyRates = new List<ExchangeRate>();
			var secondaryCurrencyRates = new List<ExchangeRate>();

			var eRef = await AvailableRatesStorage.Instance.ExchangeRateWithCurrency(referenceCurrency);
			var eSec = await AvailableRatesStorage.Instance.ExchangeRateWithCurrency(secondaryCurrency);


			if (eRef != null)
			{
				referenceCurrencyRates.Add(eRef);
			}
			if (eSec != null)
			{
				secondaryCurrencyRates.Add(eSec);
			}


			foreach (ExchangeRate r1 in referenceCurrencyRates)
			{
				foreach (ExchangeRate r2 in secondaryCurrencyRates)
				{
					if (ExchangeRateHelper.OneMatch(r1, r2))
					{
						await AddRate(r1);
						await AddRate(r2);
						if (speed == FetchSpeedEnum.SLOW)
						{
							await Fetch();
						}
						else if (speed == FetchSpeedEnum.MEDIUM)
						{
							await FetchNew();
						}
						return ExchangeRateHelper.GetCombinedRate(r1, r2);
					}
				}
			}
			return null;
		}

		public async Task<ExchangeRate> GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
		{
			referenceCurrency = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(referenceCurrency));
			secondaryCurrency = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(secondaryCurrency));

			if (referenceCurrency.Equals(secondaryCurrency))
			{
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
			}

			var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

			var exists = await AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
			var existsInverse = await AvailableRatesStorage.Instance.IsAvailable(exchangeRate.GetInverse());

			if (exists || existsInverse)
			{
				await AddRate(exchangeRate);
				if (speed == FetchSpeedEnum.SLOW)
				{
					await Fetch();
				}
				else if (speed == FetchSpeedEnum.MEDIUM)
				{
					await FetchNew();
				}

				exchangeRate = (await AllElements()).Find(e => e.Equals(exchangeRate));

				if (exists)
				{
					return exchangeRate;
				}
				return exchangeRate.GetInverse();
			}
			return null;
		}

		async Task AddRate(ExchangeRate exchangeRate)
		{
			foreach (var r in await AvailableRatesStorage.Instance.Repositories())
			{
				if (r.IsAvailable(exchangeRate))
				{
					await (await r.ExchangeRateRepository()).Add(exchangeRate);
					return;
				}
			}
			await AddToLocalRepository(exchangeRate);
			await updateCache();
		}

		public async override Task<ExchangeRateRepository> GetLocalRepository()
		{
			return (await Repositories()).Find(r => r is LocalExchangeRateRepository);
		}
	}
}