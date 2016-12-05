using System;
using System.Collections.Generic;
using System.Globalization;
using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Repositories.Account
{
	public class BlockchainAccountRepository : AddressAccountRepository
	{
		const string JsonKeyBalance = "final_balance";

		public override string Description => I18N.Blockchain;

		protected override Models.Currency Currency => Models.Currency.BTC;
		public override IEnumerable<Models.Currency> SupportedCurrencies => new List<Models.Currency> { Currency };

		protected override decimal BalanceFactor => 1e8M;
		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)JObject.Parse(httpContent)[JsonKeyBalance], CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://blockchain.info/de/address/{Address}?format=json&limit=0");

		public BlockchainAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCKCHAIN_REPOSITORY, name, address) { }

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new BlockchainAccount(id, name, money, this);
	}
}