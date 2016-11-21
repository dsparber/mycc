using data.repositories.account;
using message;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCryptos.data.repositories.account
{
    public abstract class AddressAndCoinAccountRepository : OnlineAccountRepository
    {
        readonly string address;
        Currency coin;

        protected abstract Uri GetUrl(Currency currency, string address);

        const int BUFFER_SIZE = 256000;
        readonly HttpClient client;

        public override string Data { get { return JsonConvert.SerializeObject(new KeyData(coin, address)); } }

        public AddressAndCoinAccountRepository(int type, string name, string data) : this(type, name)
        {
            var keyData = JsonConvert.DeserializeObject<KeyData>(data);

            coin = keyData.coin;
            address = keyData.address;
        }

        public AddressAndCoinAccountRepository(int type, string name, Currency coin, string address) : this(type, name)
        {
            this.coin = coin;
            this.address = address;
        }

        AddressAndCoinAccountRepository(int type, string name) : base(type, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        async Task<Money> getMoney()
        {
            var uri = GetUrl(coin, address);

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var balance = decimal.Parse(content);
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
                    var existing = Elements.FirstOrDefault();

                    var name = string.Format("{0} {1}", Name, I18N.Account);

                    if (existing != null)
                    {
                        existing = new Account(existing.Id, existing.RepositoryId, name, money);
                        await Update(existing);
                    }
                    else
                    {
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

        public sealed override string Description { get { return string.Format("{0} ({1})", DescriptionName, coin.Code); } }

        protected abstract string DescriptionName { get; }
    }
}
