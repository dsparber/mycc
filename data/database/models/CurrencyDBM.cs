using System.Threading.Tasks;
using data.database.interfaces;
using models;
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

		public CurrencyDBM(Currency currency, int repositoryId)
		{
			if (currency.Id.HasValue)
			{
				Id = currency.Id.Value;
			}
			Name = currency.Name;
			Code = currency.Code;
			RepositoryId = repositoryId;
		}

		public Task<Currency> Resolve()
		{
			return Task.Factory.StartNew(() => new Currency(Id, Code, Name));
		}
	}
}

