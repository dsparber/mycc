using System;
using System.Collections.Generic;
using System.Globalization;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
	public class EthereumAccountRepository : AddressAccountRepository
	{
		const string JsonKeyBalance = "balance";
		const string JsonKeyData = "data";

		public override string Description => I18N.Etherchain;

		protected override Currency.Model.Currency Currency => CurrencyStorage.Instance.AllElements.Find(c => c?.Code.Equals("ETH") ?? false);
		public override IEnumerable<Currency.Model.Currency> SupportedCurrencies => new List<Currency.Model.Currency> { Currency };

		protected override decimal BalanceFactor => 1e18M;
		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)(JArray.Parse(httpContent)[0] as JObject)[JsonKeyBalance], CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://etherchain.org/api/account/{Currency}");


		public EthereumAccountRepository(int id, string name, string address) : base(id, name, address) { }
		public override int RepositoryTypeId => AccountRepositoryDbm.DB_TYPE_ETHEREUM_REPOSITORY;

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new EthereumAccount(id, name, money, this);
	}
}