using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
	[Table("Currencies")]
	public class CurrencyDbm : IEntityRepositoryIdDBM<Model.Currency, string>
	{
		public CurrencyDbm() { }

		[Ignore]
		public int ParentId
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		[PrimaryKey, Column("Code")]
		public string Id { get; set; }

		[Column("Name")]
		public string Name { get; set; }

		public CurrencyDbm(Model.Currency currency)
		{
			Name = currency.Name;
			Id = currency.Code;
		}

		public Task<Model.Currency> Resolve()
		{
			return Task.Factory.StartNew(() => new Model.Currency(Id, Name));
		}
	}
}

