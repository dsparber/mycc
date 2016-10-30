using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.database.models;
using data.repositories.exchangerate;
using enums;
using MyCryptos.models;
using MyCryptos.models.helper;

namespace data.storage
{
	public class ExchangeRateStorage : AbstractDatabaseStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate, string>
	{
		public ExchangeRateStorage() : base(new ExchangeRateRepositoryDatabase()) { }


		protected override async Task OnFirstLaunch()
		{
			await Add(new BittrexExchangeRateRepository(null));
			await Add(new BtceExchangeRateRepository(null));
			await Add(new LocalExchangeRateRepository(null));
			await Add(new CryptonatorExchangeRateRepository(null));
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
			await Task.WhenAll(Repositories.Select(x => x.FetchNew()));
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

			var eRef = AvailableRatesStorage.Instance.ExchangeRateWithCurrency(referenceCurrency);
			var eSec = AvailableRatesStorage.Instance.ExchangeRateWithCurrency(secondaryCurrency);


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
			if (referenceCurrency.Equals(secondaryCurrency))
			{
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
			}

			var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

			var exists = AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
			var existsInverse = AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse);

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

				exchangeRate = Find(exchangeRate);

				if (exists)
				{
					return exchangeRate;
				}
				return exchangeRate.Inverse;
			}
			return null;
		}

		async Task AddRate(ExchangeRate exchangeRate)
		{
			var added = false;
			foreach (var r in AvailableRatesStorage.Instance.Repositories)
			{
				if (!added && r.IsAvailable(exchangeRate))
				{
					added = true;
					exchangeRate.RepositoryId = r.Id;
					await r.ExchangeRateRepository.AddOrUpdate(exchangeRate);
				}
			}
			if (!added)
			{
				exchangeRate.RepositoryId = LocalRepository.Id;
				await LocalRepository.AddOrUpdate(exchangeRate);
			}
		}

		public override ExchangeRateRepository LocalRepository
		{
			get
			{
				return Repositories.Find(r => r is LocalExchangeRateRepository);
			}
		}
	}
}