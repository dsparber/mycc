using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using data.database.models;
using data.storage;
using models;
using MyCryptos.resources;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;

namespace data.repositories.account
{
	public class BittrexAccountRepository : OnlineAccountRepository
	{
		// TODO Load from Database
		const string API_KEY = "51bb7379ae6645af87a8005f74fd272c";
		const string API_KEY_SECRET = "8eea5afc2a7340079143b8dae4e9b46f";

		const string BASE_URL = "https://bittrex.com/api/v1.1/account/getbalances?apikey={0}&nonce={1}";
		const string SIGNING = "apisign";

		const string RESULT_KEY = "result";
		const string CURRENCY_KEY = "Currency";
		const string BALANCE_KEY = "Balance";

		const int BUFFER_SIZE = 256000;
		readonly HttpClient client;

		public BittrexAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		public override async Task Fetch()
		{
			var nounce = Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
			var uri = new Uri(string.Format(BASE_URL, API_KEY, nounce));

			byte[] keyBytes = Encoding.UTF8.GetBytes(API_KEY_SECRET);
			byte[] dataBytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);

			var algorithm = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
			var hasher = algorithm.CreateHash(keyBytes);
			hasher.Append(dataBytes);
			var hash = ByteToString(hasher.GetValueAndReset());

			client.DefaultRequestHeaders.Remove(SIGNING);
			client.DefaultRequestHeaders.Add(SIGNING, hash);

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);

				var results = (JArray)json[RESULT_KEY];

				var newAccounts = new List<Account>();

				// TODO fix
				foreach (var r in results){
					var currencyCode = (string)r[CURRENCY_KEY];
					var balance = (decimal)r[BALANCE_KEY];

					if (balance != 0) {
						
						var curr = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Code.Equals(currencyCode));

						var newAccount = new Account(InternationalisationResources.BittrexAccount, new Money(balance, curr));
						var existing = Elements.Find(a => a.Money.Currency.Equals(newAccount.Money.Currency));

						if (existing != null)
						{
							existing.Money = newAccount.Money;
							newAccounts.Add(existing);
						}
						else { 
							newAccounts.Add(newAccount);
						}
					}
				}

				Elements.Clear();
				Elements.AddRange(newAccounts);
			}

			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}

		static string ByteToString(byte[] buff)
		{
			var sBuilder = new StringBuilder();
			for (int i = 0; i < buff.Length; i++)
			{
				sBuilder.Append(buff[i].ToString("X2"));
			}
			return sBuilder.ToString().ToLower();
		}
	}
}