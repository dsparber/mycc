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
    public class BulwarkAccountRepository : AddressAccountRepository
    {
        public override string Description => ConstantNames.Bulwark;

        public override Currency Currency => CurrencyHelper.Find("BWK", true);
        public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

        protected override Func<string, decimal> Balance => httpContent => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://explorer.bulwarkcrypto.com/ext/getbalance/{Address}");


        public BulwarkAccountRepository(int id, string name, string address) : base(id, name, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBulwark;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new BulwarkAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://explorer.bulwarkcrypto.com/#/address/{Address}";

    }
}