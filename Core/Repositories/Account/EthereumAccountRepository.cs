using System;
using System.Collections.Generic;
using System.Globalization;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Resources;
using MyCryptos.Core.Storage;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Repositories.Account
{
    public class EthereumAccountRepository : AddressAccountRepository
    {
        const string JsonKeyBalance = "balance";
        const string JsonKeyData = "data";

        public override string Description => I18N.Etherchain;

        protected override Models.Currency Currency => CurrencyStorage.Instance.AllElements.Find(c => c?.Code.Equals("ETH") ?? false);
        public override IEnumerable<Models.Currency> SupportedCurrencies => new List<Models.Currency> { Currency };

        protected override decimal BalanceFactor => 1e18M;
        protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)(JArray.Parse(httpContent)[0] as JObject)[JsonKeyBalance], CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://etherchain.org/api/account/{Currency}");


        public EthereumAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_ETHEREUM_REPOSITORY, name, address) { }
    }
}