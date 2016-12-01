using data.database.models;
using MyCryptos.models;
using MyCryptos.resources;
using System;
using System.Globalization;
using System.Collections.Generic;
using data.storage;
using MyCryptos.data.repositories.currency;
using System.Linq;

namespace MyCryptos.data.repositories.account
{
	class CryptoIdAccountRepository : AddressAndCoinAccountRepository
	{
		public override string DescriptionName => I18N.CryptoId;
		public override IEnumerable<Currency> SupportedCurrencies
		{
			get
			{
				var id = CurrencyStorage.Instance.RepositoryOfType<CryptoIdCurrencyRepository>().Id;
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == id).Select(e => e.Code);
				return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c.Code)));
			}
		}

		protected override Func<string, decimal> Balance => (httpContent) => decimal.Parse(httpContent, CultureInfo.InvariantCulture);
		protected override Uri Url => new Uri($"http://chainz.cryptoid.info/{Currency}/api.dws?q=getbalance&a={Address}");

		public CryptoIdAccountRepository(string name, string data) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, data) { }
		public CryptoIdAccountRepository(string name, Currency coin, string address) : base(AccountRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name, coin, address) { }
	}
}
