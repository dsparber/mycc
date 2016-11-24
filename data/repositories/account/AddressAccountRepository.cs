using data.repositories.account;
using message;
using MyCryptos.models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using MyCryptos.resources;

namespace MyCryptos.data.repositories.account
{
	public abstract class AddressAccountRepository : OnlineAccountRepository
	{
		protected string Address;
		protected virtual decimal BalanceFactor => 1;

		protected abstract Uri Url { get; }
		protected abstract Func<string, decimal> Balance { get; }

		protected abstract Currency Currency { get; }
		public abstract IEnumerable<Currency> SupportedCurrencies { get; }

		const int BUFFER_SIZE = 256000;
		readonly HttpClient client;

		public override string Data { get { return Address; } }

		protected AddressAccountRepository(int type, string name, string address) : base(type, name)
		{
			Address = address;
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		async Task<decimal?> getBalance()
		{
			var uri = Url;

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				return Balance(content) / BalanceFactor;
			}
			return null;
		}

		public sealed override async Task<bool> Test()
		{
			try
			{
				return (await getBalance()).HasValue;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public sealed override async Task<bool> Fetch()
		{
			try
			{
				var balance = await getBalance();

				if (balance.HasValue)
				{
					var existing = Elements.FirstOrDefault();
					var money = new Money(balance.Value, Currency);
					var name = Name;


					if (existing != null)
					{
						existing = new Account(existing.Id, Id, name, money);
						await Update(existing);
					}
					else
					{
						var newAccount = new Account(name, money) { RepositoryId = Id };
						await Add(newAccount);
					}

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
				Debug.WriteLine($"Error Message:\n{e.Message}\nData:\n{e.Data}\nStack trace:\n{e.StackTrace}");
			}
			return false;
		}
	}
}
