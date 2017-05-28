using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class PoloniexAccountRepository : OnlineAccountRepository
    {
        private readonly string _apiKey;
        private readonly string _privateApiKey;

        private const string Url = "https://poloniex.com/tradingApi";
        private const string Command = "returnCompleteBalances";
        private const string HeaderSign = "Sign";
        private const string HeaderKey = "Key";

        public override string Data => JsonConvert.SerializeObject(new KeyData(_apiKey, _privateApiKey));

        public PoloniexAccountRepository(int id, string name, string data) : base(id, name)
        {
            var keys = JsonConvert.DeserializeObject<KeyData>(data);

            _apiKey = keys.Key;
            _privateApiKey = keys.PrivateKey;
        }

        public PoloniexAccountRepository(int id, string name, string apiKey, string privateApiKey) : base(id, name)
        {
            _apiKey = apiKey;
            _privateApiKey = privateApiKey;
        }

        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypePoloniexRepository;

        private async Task<JObject> GetResult()
        {
            try
            {
                var client = HttpHelper.CreateClient();

                var postData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("nonce", ApplicationSettings.PoloniexRequestNonce.ToString()),
                    new KeyValuePair<string, string>("command", Command),
                    new KeyValuePair<string, string>("account", "all")
                });

                var postString = await postData.ReadAsStringAsync();
                var keyBytes = Encoding.UTF8.GetBytes(_privateApiKey);
                var dataBytes = Encoding.UTF8.GetBytes(postString);

                var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
                var hasher = algorithm.CreateHash(keyBytes);
                hasher.Append(dataBytes);
                var hash = ByteToString(hasher.GetValueAndReset());

                client.DefaultRequestHeaders.Remove(HeaderKey);
                client.DefaultRequestHeaders.Add(HeaderKey, _apiKey);
                client.DefaultRequestHeaders.Remove(HeaderSign);
                client.DefaultRequestHeaders.Add(HeaderSign, hash);

                var response = await client.PostAsync(Url, postData);

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var error = (string)json["error"];
                if (error?.StartsWith("Nonce must be greater than ") ?? false)
                {
                    var nonceString = error.Substring(27, error.IndexOf('.') - 27);
                    if (int.TryParse(nonceString, out var nonce))
                    {
                        ApplicationSettings.PoloniexRequestNonce = nonce + 1;
                        return await GetResult();
                    }
                }
                error?.LogInfo();

                return json["error"] == null ? json : null;
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
                var currencyCode = r.Key;
                var balance = r.Value["available"].ToDecimal() + r.Value["onOrders"].ToDecimal();

                if (balance == null || balance == 0) continue;

                var curr = CurrencyHelper.Find(currencyCode, true);

                var money = new Money(balance.Value, curr);
                var existing = Elements.ToList().Find(a => a.Money.Currency.Equals(money.Currency));

                var newAccount = new PoloniexAccount(existing?.Id, Name, money, existing?.IsEnabled ?? true, DateTime.Now, this);

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
            Func<FunctionalAccount, bool> notInCurrentAccounts = e => !currentAccounts.Select(a => a.Id).Contains(e.Id);
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

        public override string Description => ConstantNames.Poloniex;
    }
}