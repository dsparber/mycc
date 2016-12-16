using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Repositories.Base
{
	public abstract class AddressAccountRepository : OnlineAccountRepository
	{
		public string Address;
		protected virtual decimal BalanceFactor => 1;

		protected abstract Uri Url { get; }
		protected abstract Func<string, decimal> Balance { get; }

		protected abstract Currency.Model.Currency Currency { get; }
		public abstract IEnumerable<Currency.Model.Currency> SupportedCurrencies { get; }

		const int BUFFER_SIZE = 256000;
		readonly HttpClient client;

		public override string Data => Address;

		protected AddressAccountRepository(int id, string name, string address) : base(id, name)
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

			await Task.WhenAll(Elements.Where(a => a.Id != newAccount.Id).Select(async a => await Remove(a)));


			LastFetch = DateTime.Now;
			return true;
		}

		protected abstract FunctionalAccount GetAccount(int? id, string name, Money money);

		public override string Info => $"{I18N.Address}: {Address}";
	}
}
