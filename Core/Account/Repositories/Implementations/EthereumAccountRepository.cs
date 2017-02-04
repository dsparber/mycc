using System;
using System.Collections.Generic;
using System.Globalization;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Account.Repositories.Implementations
{
	public class EthereumAccountRepository : AddressAccountRepository
	{
		private const string JsonKeyBalance = "balance";
		private const string JsonKeyData = "data";

		public override string Description => I18N.Etherchain;

		public override Currency.Model.Currency Currency => CurrencyStorage.Instance.AllElements.Find(c => c?.Code.Equals("ETH") ?? false);
		public override IEnumerable<Currency.Model.Currency> SupportedCurrencies => new List<Currency.Model.Currency> { Currency };

		protected override decimal BalanceFactor => 1e18M;
		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)(JObject.Parse(httpContent)[JsonKeyData])[0][JsonKeyBalance], CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://etherchain.org/api/account/{Address}");


		public EthereumAccountRepository(int id, string name, string address) : base(id, name, address) { }
		public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeEthereumRepository;

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new EthereumAccount(id, name, money, this);
	}
}