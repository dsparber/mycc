using System;
using System.Collections.Generic;
using System.Globalization;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
	public class BlockchainAccountRepository : AddressAccountRepository
	{
		private const string JsonKeyBalance = "final_balance";

		public override string Description => I18N.Blockchain;

		protected override Currency.Model.Currency Currency => Core.Currency.Model.Currency.BTC;
		public override IEnumerable<Currency.Model.Currency> SupportedCurrencies => new List<Currency.Model.Currency> { Currency };

		protected override decimal BalanceFactor => 1e8M;
		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse((string)JObject.Parse(httpContent)[JsonKeyBalance], CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://blockchain.info/de/address/{Address}?format=json&limit=0");

		public BlockchainAccountRepository(int id, string name, string address) : base(id, name, address) { }
		public override int RepositoryTypeId => AccountRepositoryDbm.DB_TYPE_BLOCKCHAIN_REPOSITORY;

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new BlockchainAccount(id, name, money, this);
	}
}