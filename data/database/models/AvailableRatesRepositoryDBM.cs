using System;
using System.Threading.Tasks;
using data.database.interfaces;
using data.repositories.availablerates;
using SQLite;

namespace data.database.models
{
	[Table("AvailableRatesRepositories")]
	public class AvailableRatesRepositoryDBM : IEntityDBM<AvailableRatesRepository>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set; }

		public AvailableRatesRepositoryDBM(){}

		public AvailableRatesRepositoryDBM(AvailableRatesRepository repository)
		{
			Name = repository.Name;
			Type = repository.RepositoryTypeId;
			Id = repository.Id.HasValue ? repository.Id.Value : default(int);
		}

		public Task<AvailableRatesRepository> Resolve()
		{
			return Task.Factory.StartNew<AvailableRatesRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalAvailableRatesRepository(Name);
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexAvailableRatesRepository(Name);
					case DB_TYPE_BTCE_REPOSITORY: return new BtceAvailableRatesRepository(Name);
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorAvailableRatesRepository(Name);
					default: throw new NotImplementedException();
				}
			});
		}
	}
}

