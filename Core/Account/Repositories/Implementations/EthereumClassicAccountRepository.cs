using System;
using System.Collections.Generic;
using System.Globalization;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class EthereumClassicAccountRepository : AddressAccountRepository
    {
        private const string JsonKeyBalance = "balance";

        public override string Description => I18N.EtcChain;

        public override Currencies.Model.Currency Currency => CurrencyStorage.Find("ETC", true);
        public override IEnumerable<Currencies.Model.Currency> SupportedCurrencies => new[] { Currency };

        protected override Func<string, decimal> Balance => httpContent => decimal.Parse((string)JObject.Parse(httpContent)[JsonKeyBalance], CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://etcchain.com/api/v1/getAddressBalance?address={Address}");


        public EthereumClassicAccountRepository(int id, string name, string address) : base(id, name, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeEthereumClassicRepository;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new EthereumClassicAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://etcchain.com/addr/{Address}";

    }
}