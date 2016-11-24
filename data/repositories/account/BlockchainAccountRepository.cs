using System;
using System.Collections.Generic;
using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.data.repositories.account
{
	public class BlockchainAccountRepository : AddressAccountRepository
	{
		const string JsonKeyBalance = "final_balance";

		public override string Description => I18N.Blockchain;

		protected override Currency Currency => Currency.BTC;
		public override IEnumerable<Currency> SupportedCurrencies => new List<Currency> { Currency };

		protected override decimal BalanceFactor => 1e8M;
		protected override Func<string, decimal> Balance => (httpContent) => (decimal)JObject.Parse(httpContent)[JsonKeyBalance];
		protected override Uri Url => new Uri($"https://blockchain.info/de/address/{Address}?format=json&limit=0");

		public BlockchainAccountRepository(string name, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCKCHAIN_REPOSITORY, name, address) { }
	}
}