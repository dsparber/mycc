using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using message;
using MyCryptos.models;
using Xamarin.Forms;

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

		async Task fetch(Func<ExchangeRate, bool> filter)
		{
			var newElements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).Where(filter).ToList();

			var t = new List<Task>();
			foreach (var e in newElements)
			{
				t.Add(GetFetchTask(e));
			}
			await Task.WhenAll(t);

			await Task.WhenAll(newElements.Select(e => AddOrUpdate(e)));

			LastFetch = DateTime.Now;
		}

		public override async Task<bool> Fetch()
		{
			try
			{
				await fetch(e => true);
				return true;
			}
			catch (Exception e)
			{
				if (e is TaskCanceledException || e is WebException)
				{
					MessagingCenter.Send(e, MessageConstants.NetworkError);
				}
				else {
					Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				}
				return false;
			}
		}

		public override async Task<bool> FetchNew()
		{
			try
			{
				await fetch(e => !e.Rate.HasValue);
				return true;
			}
			catch (Exception e)
			{
				if (e is TaskCanceledException || e is WebException)
				{
					MessagingCenter.Send(e, MessageConstants.NetworkError);
				}
				else {
					Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				}
				return false;
			}
		}
	}
}

