using data.database.models;
using data.repositories.account;
using message;
using MyCryptos.models;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCryptos.data.repositories.account
{
    public abstract class SimpleJsonAccountRepository : OnlineAccountRepository
    {
        readonly string Address;

        protected abstract string BaseUrl { get; }
        protected abstract decimal BalanceFactor { get; }
        protected abstract Currency Currency { get; }
        public abstract string AccountName { get; }

        protected abstract decimal GetBalance(JObject json);

        const int BUFFER_SIZE = 256000;
        readonly HttpClient client;

        public override string Data { get { return Address; } }

        public SimpleJsonAccountRepository(int type, string name, string address) : base(type, name)
        {
            Address = address;
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        async Task<decimal?> getBalance()
        {
            var uri = new Uri(string.Format(BaseUrl, Address));

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(content);
                var balance = GetBalance(json);
                return balance / BalanceFactor;
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
                    var existing = Elements.FirstOrDefault();
                    var money = new Money(balance.Value, Currency);
                    var name = AccountName;


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
    }
}
