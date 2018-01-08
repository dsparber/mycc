using System;
using System.Collections.Generic;
using System.Globalization;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class EthereumAccountRepository : AddressAccountRepository
    {
        private const string JsonKeyBalance = "balance";

        public override string Description => ConstantNames.Etherchain;

        public override Currency Currency => CurrencyHelper.Find("ETH", true);
        public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

        protected override decimal BalanceFactor => 1e18M;
        protected override Func<string, decimal> Balance => httpContent => decimal.Parse((string)JObject.Parse(httpContent)[JsonKeyBalance], CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://etherchain.org/api/account/{Address}");


        public EthereumAccountRepository(int id, string name, string address) : base(id, name, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeEthereumRepository;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new EthereumAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://etherchain.org/account/{Address}";

    }
}