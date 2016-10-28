using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using message;
using MyCryptos.models;
using Xamarin.Forms;

namespace data.repositories.currency
{
	public abstract class OnlineCurrencyRepository : CurrencyRepository
	{
		protected OnlineCurrencyRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		protected abstract Task<IEnumerable<Currency>> GetCurrencies();

		public override async Task<bool> Fetch()
		{
			try
			{
				var currentElements = await GetCurrencies();

				if (currentElements != null)
				{
					foreach (var c in currentElements)
					{
						var existing = Elements.ToList().Find(e => e.Equals(c));

						if (existing != null)
						{
							existing.Name = c.Name;
							await Update(existing);
						}
						else {
							await Add(c);
						}
					}

					await Task.WhenAll(Elements.Where(e => !currentElements.Contains(e)).Select(e => Remove(e)));

					LastFetch = DateTime.Now;
					return true;
				}
			}
			catch (WebException e)
			{
				MessagingCenter.Send(e, MessageConstants.NetworkError);
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
			}
			return false;
		}
	}
}

