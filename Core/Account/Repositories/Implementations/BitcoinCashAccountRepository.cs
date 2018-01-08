using System;
using System.Collections.Generic;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class BitcoinCashAccountRepository : AddressAccountRepository
    {
        public BitcoinCashAccountRepository(int id, string name, string address) : base(id, name, address) { }

        public override string Description => ConstantNames.CashExplorer;
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBitcoinCashRepository;

        public override Currency Currency => new Currency("BCH", "Bitcoin Cash", true);
        public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

        protected override Func<string, decimal> Balance => httpContent => JObject.Parse(httpContent)["data"][0]["sum_value_unspent"].ToDecimal() ?? 0;
        protected override decimal BalanceFactor => 1e8M;

        protected override Uri Url => new Uri($"https://api.blockchair.com/bitcoin-cash/dashboards/address/{Address}");

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new BitcoinCashAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://blockchair.com/bitcoin-cash/address/{Address}";
    }
}