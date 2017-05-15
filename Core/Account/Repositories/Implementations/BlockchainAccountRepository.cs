using System;
using System.Collections.Generic;
using System.Globalization;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class BlockchainAccountRepository : AddressAccountRepository
    {
        private const string JsonKeyBalance = "final_balance";

        public override string Description => I18N.Blockchain;

        public override Currencies.Model.Currency Currency => Currencies.CurrencyConstants.Btc;
        public override IEnumerable<Currencies.Model.Currency> SupportedCurrencies => new List<Currencies.Model.Currency> { Currency };

        protected override decimal BalanceFactor => 1e8M;
        protected override Func<string, decimal> Balance => httpContent => decimal.Parse((string)JObject.Parse(httpContent)[JsonKeyBalance], CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://blockchain.info/de/address/{Address}?format=json&limit=0");

        public BlockchainAccountRepository(int id, string name, string address) : base(id, name, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBlockchainRepository;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new BlockchainAccount(id, name, money, isEnabled, DateTime.Now, this);
        public override string WebUrl => $"https://blockchain.info/address/{Address}";
    }
}