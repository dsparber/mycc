using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Currency.Repositories;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
	public class BlockExpertsAccountRepository : AddressAndCoinAccountRepository
	{
		public override string DescriptionName => I18N.BlockExperts;
		public override IEnumerable<Currency.Model.Currency> SupportedCurrencies
		{
			get
			{
				var id = CurrencyStorage.Instance.RepositoryOfType<BlockExpertsCurrencyRepository>().Id;
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
				return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code)));
			}
		}

		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"https://www.blockexperts.com/api?coin={Currency.Code.ToLower()}&action=getbalance&address={Address}");


		public BlockExpertsAccountRepository(int id, string name, string data) : base(id, name, data) { }
		public BlockExpertsAccountRepository(int id, string name, Currency.Model.Currency coin, string address) : base(id, name, coin, address) { }
		public override int RepositoryTypeId => AccountRepositoryDbm.DB_TYPE_BLOCK_EXPERTS_REPOSITORY;

		protected override FunctionalAccount GetAccount(int? id, string name, Money money) => new BlockExpertsAccount(id, name, money, this);
	}
}