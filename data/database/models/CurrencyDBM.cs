using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;
using SQLite;
using System;

namespace data.database.models
{
	[Table("Currencies")]
	public class CurrencyDBM : IEntityRepositoryIdDBM<Currency, string>
	{
		public CurrencyDBM() { }

		[Ignore]
		public int RepositoryId
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public string Name { get; set; }

		[PrimaryKey, Column("_id")]
		public string Id { get; set; }

		public CurrencyDBM(Currency currency)
		{
			Name = currency.Name;
			Id = currency.Code;
		}

		public Task<Currency> Resolve()
		{
			return Task.Factory.StartNew(() => new Currency(Id, Name));
		}
	}
}

