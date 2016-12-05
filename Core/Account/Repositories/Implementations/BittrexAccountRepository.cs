using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
    public class BittrexAccountRepository : OnlineAccountRepository
    {
        // Test Data
        // const string API_KEY = "51bb7379ae6645af87a8005f74fd272c";
        // const string API_KEY_SECRET = "8eea5afc2a7340079143b8dae4e9b46f";

        string apiKey, apiKeyPrivate;

        const string BASE_URL = "https://bittrex.com/api/v1.1/account/getbalances?apikey={0}&nonce={1}";
        const string SIGNING = "apisign";

        const string RESULT_KEY = "result";
        const string CURRENCY_KEY = "Currency";
        const string BALANCE_KEY = "Balance";

        const int BUFFER_SIZE = 256000;
        readonly HttpClient client;

        public override string Data { get { return JsonConvert.SerializeObject(new KeyData(apiKey, apiKeyPrivate)); } }

        public BittrexAccountRepository(string name, string data) : this(name)
        {
            var keys = JsonConvert.DeserializeObject<KeyData>(data);

            apiKey = keys.key;
            apiKeyPrivate = keys.privateKey;
        }

        public BittrexAccountRepository(string name, string apiKey, string apiKeyPrivate) : this(name)
        {
            this.apiKey = apiKey;
            this.apiKeyPrivate = apiKeyPrivate;
        }

        BittrexAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        async Task<JArray> getResult()
        {
            var nounce = Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
            var uri = new Uri(string.Format(BASE_URL, apiKey, nounce));

            var keyBytes = Encoding.UTF8.GetBytes(apiKeyPrivate);
            var dataBytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);

            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
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
                return results;
            }
            return null;
        }

        public override async Task<bool> Test()
        {
            try
            {
                return (await getResult()) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> FetchOnline()
        {
            var results = await getResult();

            if (results == null) return false;

            var currentAccounts = new List<FunctionalAccount>();

            foreach (var r in results)
            {
                var currencyCode = (string)r[CURRENCY_KEY];
                var balance = decimal.Parse((string)r[BALANCE_KEY], CultureInfo.InvariantCulture);

                if (balance != 0)
                {

                    var curr = CurrencyStorage.Instance.Find(new Currency.Model.Currency(currencyCode));

                    var money = new Money(balance, curr);
                    var existing = Elements.ToList().Find(a => a.Money.Currency.Equals(money.Currency));

                    var newAccount = new BittrexAccount(existing?.Id, $"{Name} ({curr.Code})", money, this);

                    if (existing != null)
                    {
                        await Update(newAccount);
                    }
                    else
                    {
                        await Add(newAccount);
                    }
                    currentAccounts.Add(newAccount);
                }
            }
            Func<FunctionalAccount, bool> notInCurrentAccounts = e => !currentAccounts.Select(a => a.Money.Currency).Contains(e.Money.Currency);
            await Task.WhenAll(Elements.Where(notInCurrentAccounts).Select(Remove));

            LastFetch = DateTime.Now;
            return true;
        }

        static string ByteToString(byte[] buff)
        {
            var sBuilder = new StringBuilder();
            for (var i = 0; i < buff.Length; i++)
            {
                sBuilder.Append(buff[i].ToString("X2"));
            }
            return sBuilder.ToString().ToLower();
        }

        class KeyData
        {

            public string key, privateKey;

            public KeyData(string key, string privateKey)
            {
                this.key = key;
                this.privateKey = privateKey;
            }
        }

        public override string Description { get { return I18N.Bittrex; } }
    }
}