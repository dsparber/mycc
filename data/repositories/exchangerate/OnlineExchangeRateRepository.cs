using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
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

		public override async Task<bool> Fetch()
		{
			try
			{
				Elements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).ToList();

				var t = new List<Task>();
				foreach (var e in Elements) t.Add(GetFetchTask(e));
				await Task.WhenAll(t);

				await WriteToDatabase();
				LastFetch = DateTime.Now;
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}

		public override async Task<bool> FetchNew()
		{
			try
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
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}
	}
}

