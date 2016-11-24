using System;
using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;

using MyCryptos.data.repositories.account;
using System.Collections.Generic;
using data.storage;
using data.repositories.currency;
using System.Linq;

namespace data.repositories.account
{
	public class BlockExpertsAccountRepository : AddressAndCoinAccountRepository
	{
		public override string DescriptionName => I18N.BlockExperts;
		public override IEnumerable<Currency> SupportedCurrencies
		{
			get
			{
				var id = CurrencyStorage.Instance.RepositoryOfType<BlockExpertsCurrencyRepository>().Id;
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == id).Select(e => e.Code);
				return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c.Code)));
			}
		}

		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent);
		protected override Uri Url => new Uri($"https://www.blockexperts.com/api?coin={Currency}&action=getbalance&address={Address}");


		public BlockExpertsAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, data) { }
		public BlockExpertsAccountRepository(string name, Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY, name, coin, address) { }

	}
}