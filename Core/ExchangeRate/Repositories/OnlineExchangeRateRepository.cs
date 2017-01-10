using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int id) : base(id) { }

		public override async Task<bool> LoadFromDatabase()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		protected abstract Task GetFetchTask(Model.ExchangeRate exchangeRate);

		private async Task FetchWithFilter(Func<Model.ExchangeRate, bool> filter)
		{
			var newElements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).Where(filter).ToList();

			try
			{
				await Task.WhenAll(newElements.Select(GetFetchTask));
				await Task.WhenAll(newElements.Select(AddOrUpdate));
			}
			catch (Exception e)
			{
				throw e;
			}

			LastFetch = DateTime.Now;
		}

		public override async Task<bool> FetchOnline()
		{
			await FetchWithFilter(e => true);
			return true;
		}

		public override async Task<bool> FetchNew()
		{
			await FetchWithFilter(e => !e.Rate.HasValue);
			return true;
		}


		public async Task FetchOnline(List<ExchangeRate.Model.ExchangeRate> neededRates)
		{
			await FetchWithFilter(neededRates.Contains);
		}
	}
}

