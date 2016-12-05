using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Currency.Repositories;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
    public class CryptoIdAccountRepository : AddressAndCoinAccountRepository
    {
        public override string DescriptionName => I18N.CryptoId;
        public override IEnumerable<Currency.Model.Currency> SupportedCurrencies
        {
            get
            {
                var id = CurrencyStorage.Instance.RepositoryOfType<CryptoIdCurrencyRepository>().Id;
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
                return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code)));
            }
        }

        protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"http://chainz.cryptoid.info/{Currency}/api.dws?q=getbalance&a={Address}");

        public CryptoIdAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, data) { }
        public CryptoIdAccountRepository(string name, Currency.Model.Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, coin, address) { }

        protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new CryptoIdAccount(id, name, money, this);
    }
}
