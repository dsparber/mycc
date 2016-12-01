using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;
using MyCryptos.Core.Resources;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Repositories.Account
{
    public class CryptoIdAccountRepository : AddressAndCoinAccountRepository
    {
        public override string DescriptionName => I18N.CryptoId;
        public override IEnumerable<Models.Currency> SupportedCurrencies
        {
            get
            {
                var id = CurrencyStorage.Instance.RepositoryOfType<CryptoIdCurrencyRepository>().Id;
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == id).Select(e => e.Code);
                return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c.Code)));
            }
        }

        protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"http://chainz.cryptoid.info/{Currency}/api.dws?q=getbalance&a={Address}");

        public CryptoIdAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, data) { }
        public CryptoIdAccountRepository(string name, Models.Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, coin, address) { }
    }
}
