using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Currency.Repositories;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
	[Table("CurrencyRepositoryMap")]
	public class CurrencyMapRepositoryDbm : IEntityDBM<CurrencyRepositoryMap, int>
	{

		public CurrencyMapRepositoryDbm() { }

		public CurrencyMapRepositoryDbm(CurrencyRepositoryMap repository)
		{
			Id = repository.Id;
		}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public Task<CurrencyRepositoryMap> Resolve()
		{
			return Task.Factory.StartNew(() =>
			{
				return new CurrencyRepositoryMap { Id = Id };
			});
		}
	}
}

