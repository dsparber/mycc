using System.Threading.Tasks;
using data.database.interfaces;
using data.repositories.currency;
using SQLite;
namespace data.database.models
{
	[Table("CurrencyRepositoryMap")]
	public class CurrencyRepositoryMapDBM : IEntityDBM<CurrencyRepositoryMap, int>
	{

		public CurrencyRepositoryMapDBM() { }

		public CurrencyRepositoryMapDBM(CurrencyRepositoryMap repository)
		{
			Id = repository.Id;
		}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public Task<CurrencyRepositoryMap> Resolve()
		{
			return Task.Factory.StartNew(() =>
			{
				return new CurrencyRepositoryMap() { Id = Id };
			});
		}
	}
}

