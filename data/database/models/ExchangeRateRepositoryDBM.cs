using System;
using System.Threading.Tasks;
using data.database.interfaces;
using data.repositories.exchangerate;
using SQLite;

namespace data.database.models
{
	[Table("ExchangeRateRepositories")]
	public class ExchangeRateRepositoryDBM : IEntityDBM<ExchangeRateRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set; }

		public ExchangeRateRepositoryDBM() { }

		public ExchangeRateRepositoryDBM(ExchangeRateRepository repository)
		{
			Name = repository.Name;
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		public Task<ExchangeRateRepository> Resolve()
		{
			return Task.Factory.StartNew<ExchangeRateRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalExchangeRateRepository(Name) { Id = Id };
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexExchangeRateRepository(Name) { Id = Id };
					case DB_TYPE_BTCE_REPOSITORY: return new BtceExchangeRateRepository(Name) { Id = Id };
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorExchangeRateRepository(Name) { Id = Id };
					default: throw new NotImplementedException();
				}
			});
		}
	}
}

