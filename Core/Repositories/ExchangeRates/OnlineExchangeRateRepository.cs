using System;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		protected abstract Task GetFetchTask(ExchangeRate exchangeRate);

		private async Task FetchWithFilter(Func<ExchangeRate, bool> filter)
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

		public override async Task<bool> Fetch()
		{
			await FetchWithFilter(e => true);
			return true;
		}

		public override async Task<bool> FetchNew()
		{
			await FetchWithFilter(e => !e.Rate.HasValue);
			return true;
		}
	}
}

