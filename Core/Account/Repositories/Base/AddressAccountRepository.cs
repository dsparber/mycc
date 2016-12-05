using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Models;
using Xamarin.Forms;

namespace MyCryptos.Core.Repositories.Account
{
	public abstract class AddressAccountRepository : OnlineAccountRepository
	{
		protected string Address;
		protected virtual decimal BalanceFactor => 1;

		protected abstract Uri Url { get; }
		protected abstract Func<string, decimal> Balance { get; }

		protected abstract Models.Currency Currency { get; }
		public abstract IEnumerable<Models.Currency> SupportedCurrencies { get; }

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

		public sealed override async Task<bool> FetchOnline()
		{

			var balance = await getBalance();

			if (!balance.HasValue) return false;

			var existing = Elements.FirstOrDefault();
			var money = new Money(balance.Value, Currency);
			var name = Name;

			var newAccount = GetAccount(existing?.Id ?? default(int), name, money);

			if (existing != null)
			{
				await Update(newAccount);
			}
			else
			{
				await Add(newAccount);
			}

			LastFetch = DateTime.Now;
			return true;
		}

		protected abstract FunctionalAccount GetAccount(int? id, string name, Money money);
	}
}
