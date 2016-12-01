using System;
using System.Globalization;
using System.Collections.Generic;
using data.database.models;
using data.storage;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.data.repositories.account
{
	public class EthereumAccountRepository : AddressAccountRepository
	{
		const string JsonKeyBalance = "balance";
		const string JsonKeyData = "data";

		public override string Description => I18N.Etherchain;

		protected override Currency Currency => CurrencyStorage.Instance.AllElements.Find(c => c.Code.Equals("ETH"));
		public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

		protected override decimal BalanceFactor => 1e18M;
		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)(JArray.Parse(httpContent)[0] as JObject)[JsonKeyBalance], CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://etherchain.org/api/account/{Currency}");


		public EthereumAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_ETHEREUM_REPOSITORY, name, address) { }
	}
}