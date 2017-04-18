using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Resources;

namespace MyCC.Core.Account.Repositories.Implementations
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

        public override async Task<bool> Test()
        {
            var uri = new Uri($"https://chainz.cryptoid.info/{Currency.Code.ToLower()}/api.dws?q=addressfirstseen&a={Address}");
            HttpResponseMessage response;
            if (PostContent == null)
            {
                response = await Client.GetAsync(uri);
            }
            else
            {
                response = await Client.PostAsync(uri, PostContent);
            }

            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            return !content.StartsWith("ERROR");
        }

        protected override Func<string, decimal> Balance => httpContent => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
        protected override Uri Url => new Uri($"https://chainz.cryptoid.info/{Currency.Code.ToLower()}/api.dws?q=getbalance&a={Address}");

        public CryptoIdAccountRepository(int id, string name, string data) : base(id, name, data) { }
        public CryptoIdAccountRepository(int id, string name, Currency.Model.Currency coin, string address) : base(id, name, coin, address) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeCryptoidRepository;

        protected override FunctionalAccount GetAccount(int? id, string name, Money money, bool isEnabled) => new CryptoIdAccount(id, name, money, isEnabled, DateTime.Now, this);
        public override string WebUrl => $"https://chainz.cryptoid.info/{Currency.Code.ToLower()}/address.dws?{Address}.htm";
    }
}
