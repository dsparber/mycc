﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using models;
using data.storage;
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

        public override async Task<bool> Fetch()
        {
            var uri = new Uri(string.Format(BASE_URL, address));

            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {

                    var content = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(content);
                    var balance = (decimal)json[JSON_BALANCE];

                    var btc = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(Currency.BTC));

                    var name = (Elements.Count > 0) ? Elements.First().Name : InternationalisationResources.BlockchainAccount;
                    var newAccount = new Account(name, new Money(balance, btc));

                    await DeleteAll();
                    Elements.Clear();
                    Elements.Add(newAccount);

                    await WriteToDatabase();
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