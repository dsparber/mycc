using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class BlockchainXpubAccountRepository : AddressAccountRepository
    {
        private const string AddressKey = "addr";
        private const string ResponseKey = "response";
        private const string ConfirmedKey = "confirmed";

        private const string Authorization = "Authorization";
        private const string AuthorizationKey = "Bearer HzVNqyM0jFVjeLCiMK47w99GDf14SBcy6feDxhKcXyQ";

        public BlockchainXpubAccountRepository(int id, string name, string address) : base(id, name, address)
        {
            Client.DefaultRequestHeaders.Add(Authorization, AuthorizationKey);
        }

        public override string Description => ConstantNames.Blockchain;
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBlockchainXpubRepository;

        public override Currency Currency => Currencies.CurrencyConstants.Btc;
        public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

        protected override Func<string, decimal> Balance => httpContent => JObject.Parse(httpContent)[ResponseKey].Select(d => decimal.Parse((string)d[ConfirmedKey], CultureInfo.InvariantCulture)).Sum();
        protected override decimal BalanceFactor => 1e8M;

        protected override Uri Url => new Uri("https://www.blockonomics.co/api/balance");
        protected override HttpContent PostContent => new StringContent($"{{\"{AddressKey}\":\"{Address}\"}}");

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new BlockchainXpubAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://blockchain.info/xpub/{Address}";
    }
}