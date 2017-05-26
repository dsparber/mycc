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
    public class ReddCoinAccountRepository : AddressAccountRepository
    {
        public ReddCoinAccountRepository(int id, string name, string address) : base(id, name, address) { }

        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeReddCoinRepository;
        public override string Description => ConstantNames.ReddCoin;

        public override string WebUrl => $"https://live.reddcoin.com/address/{Address}";
        protected override Uri Url => new Uri($"https://live.reddcoin.com/api/addr/{Address}?noTxList=1");

        protected override Func<string, decimal> Balance => httpContent => decimal.Parse((string)JObject.Parse(httpContent)["balance"], CultureInfo.InvariantCulture);

        public override Currency Currency => CurrencyHelper.Find("RDD", true);

        public override IEnumerable<Currency> SupportedCurrencies => new[] { Currency };

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new ReddCoinAccount(id, name, money, isEnabled, DateTime.Now, this);
    }
}