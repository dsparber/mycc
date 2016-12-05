using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.ExchangeRate.Repositories;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
	[Table("AvailableRatesRepositories")]
	public class AvailableRatesRepositoryDbm : IEntityDBM<AvailableRatesRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		[Column("Type")]
		public int Type { get; set; }

		public AvailableRatesRepositoryDbm() { }

		public AvailableRatesRepositoryDbm(AvailableRatesRepository repository)
		{
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		public Task<AvailableRatesRepository> Resolve()
		{
			return Task.Factory.StartNew<AvailableRatesRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalAvailableRatesRepository(Id);
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexAvailableRatesRepository(Id);
					case DB_TYPE_BTCE_REPOSITORY: return new BtceAvailableRatesRepository(Id);
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorAvailableRatesRepository(Id);
					default: throw new NotSupportedException();
				}
			});
		}
	}
}

