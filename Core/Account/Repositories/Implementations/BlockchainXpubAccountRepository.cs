﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
	public class BlockchainXpubAccountRepository : AddressAccountRepository
	{
		private const string addressKey = "addr";
		private const string responseKey = "response";
		private const string confirmedKey = "confirmed";

		public BlockchainXpubAccountRepository(int id, string name, string address) : base(id, name, address) { }

		public override string Description => I18N.Blockchain;
		public override int RepositoryTypeId => AccountRepositoryDbm.DB_TYPE_BLOCKCHAIN_XPUB_REPOSITORY;

		protected override Currency.Model.Currency Currency => Core.Currency.Model.Currency.Btc;
		public override IEnumerable<Currency.Model.Currency> SupportedCurrencies => new List<Currency.Model.Currency> { Currency };

		protected override Func<string, decimal> Balance => (httpContent) => JObject.Parse(httpContent)[responseKey].Select(d => decimal.Parse((string)d[confirmedKey], CultureInfo.InvariantCulture)).Sum();
		protected override decimal BalanceFactor => 1e8M;

		protected override Uri Url => new Uri("https://www.blockonomics.co/api/balance");
		protected override HttpContent PostContent => new StringContent($"{{\"{addressKey}\":\"{Address}\"}}");

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new BlockchainXpubAccount(id, name, money, this);
	}
}
