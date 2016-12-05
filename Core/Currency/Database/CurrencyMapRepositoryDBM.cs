using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Repositories.Currency;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
	[Table("CurrencyRepositoryMap")]
	public class CurrencyMapRepositoryDBM : IEntityDBM<CurrencyRepositoryMap, int>
	{

		public CurrencyMapRepositoryDBM() { }

		public CurrencyMapRepositoryDBM(CurrencyRepositoryMap repository)
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

