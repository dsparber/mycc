using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;
using data.storage;
using MyCryptos.resources;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using message;

namespace data.repositories.account
{
	public class BlockExpertsAccountRepository : OnlineAccountRepository
	{
		// Test Data
		// const string testCoin = "alx";
		// const string testAddress = "AZRRZiKXNigfakwrf7nTn2exEbcod7kjrA";

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

		async Task<Money> getMoney()
		{
			var uri = new Uri(string.Format(BASE_URL, coin.Code, address));

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{

				var content = await response.Content.ReadAsStringAsync();
				var balance = decimal.Parse(content);

				var dbCoin = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(coin));
				if (dbCoin == null)
				{
					await CurrencyStorage.Instance.AddToLocalRepository(coin);
					dbCoin = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(coin));
				}

				coin = dbCoin ?? coin;
				var money = new Money(balance, coin);
				return money;
			}
			return null;
		}

		public override async Task<bool> Fetch()
		{
			try
			{
				var money = await getMoney();

				if (money != null)
				{
					var existing = Elements.First();

					if (existing != null)
					{
						existing.Money = money;
						await Update(existing);
					}
					else {
						var newAccount = new Account(coin.Name, money);
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

		public override async Task<bool> Test()
		{
			try
			{
				return (await getMoney()) != null;
			}
			catch (Exception)
			{
				return false;
			}
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

		public override string Description { get { return string.Format("{0} / {1}", InternationalisationResources.BlockExperts, coin.Name); } }
	}
}