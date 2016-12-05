using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Currency;
using MyCryptos.Core.Resources;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Repositories.Account
{
	public class BlockExpertsAccountRepository : AddressAndCoinAccountRepository
	{
		public override string DescriptionName => I18N.BlockExperts;
		public override IEnumerable<Models.Currency> SupportedCurrencies
		{
			get
			{
				var id = CurrencyStorage.Instance.RepositoryOfType<BlockExpertsCurrencyRepository>().Id;
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
				return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code)));
			}
		}

		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://www.blockexperts.com/api?coin={Currency}&action=getbalance&address={Address}");


		public BlockExpertsAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, data) { }
		public BlockExpertsAccountRepository(string name, Models.Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, coin, address) { }

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new BlockExpertsAccount(id, name, money, this);
	}
}