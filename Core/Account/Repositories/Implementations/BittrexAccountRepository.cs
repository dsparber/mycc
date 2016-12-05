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

        private readonly string apiKey;
        private readonly string privateApiKey;

        private const string BaseUrl = "https://bittrex.com/api/v1.1/account/getbalances?apikey={0}&nonce={1}";
        private const string Signing = "apisign";

        private const string ResultKey = "result";
        private const string CurrencyKey = "Currency";
        private const string BalanceKey = "Balance";

        private const int BufferSize = 256000;

        public override string Data => JsonConvert.SerializeObject(new KeyData(apiKey, privateApiKey));

        public BittrexAccountRepository(string name, string data) : this(name)
        {
            var keys = JsonConvert.DeserializeObject<KeyData>(data);

            apiKey = keys.Key;
            privateApiKey = keys.PrivateKey;
        }

        public BittrexAccountRepository(string name, string apiKey, string privateApiKey) : this(name)
        {
            this.apiKey = apiKey;
            this.privateApiKey = privateApiKey;
        }

        private BittrexAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name) { }

        public async Task<JArray> GetResult(Currency.Model.Currency currency = null)
        {
            var nounce = Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
            var uri = new Uri($"{string.Format(BaseUrl, apiKey, nounce)}{(currency != null ? $"&currency={currency.Code}" : string.Empty)}");

            var keyBytes = Encoding.UTF8.GetBytes(privateApiKey);
            var dataBytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);

            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
            var hasher = algorithm.CreateHash(keyBytes);
            hasher.Append(dataBytes);
            var hash = ByteToString(hasher.GetValueAndReset());

            var client = new HttpClient { MaxResponseContentBufferSize = BufferSize };

            client.DefaultRequestHeaders.Remove(Signing);
            client.DefaultRequestHeaders.Add(Signing, hash);

            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var results = (JArray)json[ResultKey];
            return results;
        }

        public override async Task<bool> Test()
        {
            try
            {
                return (await GetResult()) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> FetchOnline()
        {
            var results = await GetResult();

            if (results == null) return false;

            var currentAccounts = new List<FunctionalAccount>();

            foreach (var r in results)
            {
                var currencyCode = (string)r[CurrencyKey];
                var balance = decimal.Parse((string)r[BalanceKey], CultureInfo.InvariantCulture);

                if (balance == 0) continue;

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
            Func<FunctionalAccount, bool> notInCurrentAccounts = e => !currentAccounts.Select(a => a.Money.Currency).Contains(e.Money.Currency);
            await Task.WhenAll(Elements.Where(notInCurrentAccounts).Select(Remove));

            LastFetch = DateTime.Now;
            return true;
        }

        private static string ByteToString(IEnumerable<byte> buff)
        {
            var sBuilder = new StringBuilder();
            foreach (var t in buff)
            {
                sBuilder.Append(t.ToString("X2"));
            }
            return sBuilder.ToString().ToLower();
        }

        private class KeyData
        {

            public readonly string Key;
            public readonly string PrivateKey;

            public KeyData(string key, string privateKey)
            {
                Key = key;
                PrivateKey = privateKey;
            }
        }

        public override string Description => I18N.Bittrex;
    }
}