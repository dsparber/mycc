using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;
using SQLite;
namespace data.database.models
{
	[Table("Currencies")]
	public class CurrencyDBM : IEntityRepositoryIdDBM<Currency>
	{
		public CurrencyDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		[MaxLength(3)]
		public string Code { get; set; }

		public int RepositoryId { get; set; }

		public CurrencyDBM(Currency currency)
		{
			Id = currency.Id.GetValueOrDefault();
			Name = currency.Name;
			Code = currency.Code;
			RepositoryId = currency.RepositoryId.GetValueOrDefault();
		}

		public Task<Currency> Resolve()
		{
			return Task.Factory.StartNew(() => new Currency(Id, Code, Name));
		}
	}
}

