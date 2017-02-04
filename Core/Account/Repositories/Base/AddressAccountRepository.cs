using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Resources;

namespace MyCC.Core.Account.Repositories.Base
{
	public abstract class AddressAccountRepository : OnlineAccountRepository
	{
		public string Address;
		protected virtual decimal BalanceFactor => 1;
		protected virtual HttpContent PostContent => null;

		protected abstract Uri Url { get; }
		protected abstract Func<string, decimal> Balance { get; }

		public abstract Currency.Model.Currency Currency { get; }
		public abstract IEnumerable<Currency.Model.Currency> SupportedCurrencies { get; }

		private const int BufferSize = 256000;
		private readonly HttpClient _client;

		public override string Data => Address;

		protected AddressAccountRepository(int id, string name, string address) : base(id, name)
		{
			Address = address;
			_client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
		}

		private async Task<decimal?> GetBalance()
		{
			var uri = Url;
			HttpResponseMessage response;
			if (PostContent == null)
			{
				response = await _client.GetAsync(uri);
			}
			else
			{
				response = await _client.PostAsync(uri, PostContent);
			}

			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			return Balance(content) / BalanceFactor;
		}

		public sealed override async Task<bool> Test()
		{
			try
			{
				return (await GetBalance()).HasValue;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public sealed override async Task<bool> FetchOnline()
		{

			var balance = await GetBalance();

			if (!balance.HasValue) return false;

			var existing = Elements.FirstOrDefault();
			Currency.IsCryptoCurrency = true;
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
