using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using message;
using Newtonsoft.Json.Linq;

namespace data.repositories.account
{
	public class BlockchainAccountRepository : OnlineAccountRepository
	{
		readonly string address;

		const string BASE_URL = "https://blockchain.info/de/address/{0}?format=json&limit=0";
		const string JSON_BALANCE = "final_balance";

		const int BUFFER_SIZE = 256000;
		readonly HttpClient client;

		public override string Data { get { return JsonConvert.SerializeObject(new KeyData(address)); } }

		public BlockchainAccountRepository(string name, string dataOrAddress) : this(name)
		{
			try
			{
				var keyData = JsonConvert.DeserializeObject<KeyData>(dataOrAddress);
				address = keyData.address;
			}
			catch
			{
				address = dataOrAddress;
			}
		}

		BlockchainAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BLOCKCHAIN_REPOSITORY, name)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		async Task<decimal?> getBalance()
		{
			var uri = new Uri(string.Format(BASE_URL, address));

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				var json = JObject.Parse(content);
				var balance = (decimal)json[JSON_BALANCE];
				return balance;
			}
			return null;
		}

		public override async Task<bool> Test()
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

		public override async Task<bool> Fetch()
		{
			try
			{
				var balance = await getBalance();

				if (balance.HasValue)
				{
					var btc = Currency.BTC;

					var existing = Elements.First();
					var money = new Money(balance.Value, btc);


					if (existing != null)
					{
						existing = new Account(existing.Id, existing.RepositoryId, existing.Name, money);
						await Update(existing);
					}
					else
					{
						var name = InternationalisationResources.BlockchainAccount;
						var newAccount = new Account(name, money);
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
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
			}
			return false;
		}

		class KeyData
		{

			public string address;

			public KeyData(string address)
			{
				this.address = address;
			}
		}

		public override string Description { get { return InternationalisationResources.Blockchain; } }
	}
}