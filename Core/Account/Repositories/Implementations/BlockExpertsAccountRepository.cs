using System;
using System.Collections.Generic;
using System.Globalization;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Resources;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class BlockExpertsAccountRepository : AddressAndCoinAccountRepository
    {
        public override string DescriptionName => I18N.BlockExperts;
        public override IEnumerable<Currencies.Model.Currency> SupportedCurrencies => CurrencyStorage.CurrenciesOf(CurrencyConstants.FlagBlockExperts);

        protected override Func<string, decimal> Balance => httpContent => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://www.blockexperts.com/api?coin={Currency.Code.ToLower()}&action=getbalance&address={Address}");


        public BlockExpertsAccountRepository(int id, string name, string data) : base(id, name, data) { }
        public BlockExpertsAccountRepository(int id, string name, Currencies.Model.Currency coin, string address) : base(id, name, coin, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeBlockExpertsRepository;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new BlockExpertsAccount(id, name, money, isEnabled, DateTime.Now, this);

        public override string WebUrl => $"https://www.blockexperts.com/{Currency.Code.ToLower()}/address/{Address}";
    }
}