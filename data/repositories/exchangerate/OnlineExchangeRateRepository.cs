using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
			LastFastFetch = DateTime.Now;
		}

		protected abstract Task GetFetchTask(ExchangeRate exchangeRate);

		public override async Task Fetch()
		{
			Elements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).ToList();

			var t = new List<Task>();
			foreach (var e in Elements) t.Add(GetFetchTask(e));
			await Task.WhenAll(t);

			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}

		public override async Task FetchNew()
		{
			Elements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).ToList();

			var t = new List<Task>();
			foreach (var e in Elements)
			{
				if (!e.Rate.HasValue)
				{
					t.Add(GetFetchTask(e));
				}
			}
			await Task.WhenAll(t);

			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}
	}
}

