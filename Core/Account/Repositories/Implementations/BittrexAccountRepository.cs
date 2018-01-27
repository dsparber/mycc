using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class BittrexAccountRepository : OnlineAccountRepository
    {
        private readonly string _apiKey;
        private readonly string _privateApiKey;

        private const string BaseUrl = "https://bittrex.com/api/v1.1/account/getbalance{2}apikey={0}&nonce={1}";
        private const string Signing = "apisign";

        private const string ResultKey = "result";
        private const string CurrencyKey = "Currency";
        private const string BalanceKey = "Balance";
        private const string KeySuccess = "success";

        private const int BufferSize = 256000;

        public override string Data => JsonConvert.SerializeObject(new KeyData(_apiKey, _privateApiKey));

        public BittrexAccountRepository(int id, string name, string data) : base(id, name)
        {
            var keys = JsonConvert.DeserializeObject<KeyData>(data);

            _apiKey = keys.Key;
            _privateApiKey = keys.PrivateKey;
        }

        public BittrexAccountRepository(int id, string name, string apiKey, string privateApiKey) : base(id, name)
        {
            _apiKey = apiKey;
            _privateApiKey = privateApiKey;
        }

        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBittrexRepository;

        public async Task<JToken> GetResult(Currency currency = null)
        {
            try
            {
                var nounce = Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
                var uri = new Uri(string.Format(BaseUrl, _apiKey, nounce, currency != null ? $"?currency={currency.Code}&" : "s?"));

                var keyBytes = Encoding.UTF8.GetBytes(_privateApiKey);
                var dataBytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);

                var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
                var hasher = algorithm.CreateHash(keyBytes);
                hasher.Append(dataBytes);
                var hash = ByteToString(hasher.GetValueAndReset());

                var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };

                client.DefaultRequestHeaders.Remove(Signing);
                client.DefaultRequestHeaders.Add(Signing, hash);


                var response = await client.GetAsync(uri);

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                if (!(bool)json[KeySuccess]) return null;

                var results = json[ResultKey];
                return results;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        public override async Task<bool> Test()
        {
            try
            {
                return await GetResult() != null;
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
                currencyCode = "BCC".Equals(currencyCode) ? "BCH" : currencyCode;
                var balance = decimal.Parse((string)r[BalanceKey], NumberStyles.Float, CultureInfo.InvariantCulture);

                if (balance == 0) continue;

                var curr = CurrencyHelper.Find(currencyCode, true);

                var money = new Money(balance, curr);
                var existing = Elements.ToList().Find(a => a.Money.Currency.Equals(money.Currency));

                var account = new BittrexAccount(existing?.Id, Name, money, existing?.IsEnabled ?? true, DateTime.Now, this);

                if (existing != null)
                {
                    await Update(account);
                }
                else
                {
                    await Add(account);
                }
                currentAccounts.Add(account);
            }
            bool NotInCurrentAccounts(FunctionalAccount e) => !currentAccounts.Select(a => a.Id).Contains(e.Id);
            await Task.WhenAll(Elements.Where(NotInCurrentAccounts).ToArray().Select(Remove));

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

        public override string Description => ConstantNames.Bittrex;
    }
}