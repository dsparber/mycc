using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using models;
using data.storage;
using MyCryptos.resources;
using Newtonsoft.Json;
using System.Diagnostics;

namespace data.repositories.account
{
	public class BlockExpertsAccountRepository : OnlineAccountRepository
	{

		const string testCoin = "alx";
		const string testAddress = "AZRRZiKXNigfakwrf7nTn2exEbcod7kjrA";

		readonly string address;
		Currency coin;

		const string BASE_URL = "https://www.blockexperts.com/api?coin={0}&action=getbalance&address={1}";

		const int BUFFER_SIZE = 256000;
		readonly HttpClient client;

		public override string Data { get { return JsonConvert.SerializeObject(new KeyData(coin, address)); } }

		public BlockExpertsAccountRepository(string name, string data) : this(name)
		{
			var keyData = JsonConvert.DeserializeObject<KeyData>(data);

			coin = keyData.coin;
			address = keyData.address;
		}

		public BlockExpertsAccountRepository(string name, Currency coin, string address) : this(name)
		{
			this.coin = coin;
			this.address = address;
		}

		BlockExpertsAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		public override async Task<bool> Fetch()
		{
			var uri = new Uri(string.Format(BASE_URL, coin.Code, address));

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				try
				{
					var content = await response.Content.ReadAsStringAsync();
					var balance = decimal.Parse(content);

					var dbCoin = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(coin));
					if (dbCoin == null) {
						await CurrencyStorage.Instance.Add(coin);
						dbCoin = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(coin));
					}

					coin = dbCoin ?? coin;

					var newAccount = new Account(Description, new Money(balance, coin));

					// Remove old
					await DeleteAll();
					Elements.Clear();

					Elements.Add(newAccount);

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
			return false;
		}

		class KeyData
		{

			public string address;
			public Currency coin;

			public KeyData(Currency coin, string address)
			{
				this.coin = coin;
				this.address = address;
			}
		}

		public override string Description { get { return string.Format("{0} / {1}", (coin.Name ?? coin.Code), InternationalisationResources.BlockExperts); } }
	}
}