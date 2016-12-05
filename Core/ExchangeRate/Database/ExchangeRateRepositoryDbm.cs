using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.ExchangeRate.Repositories;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
	[Table("ExchangeRateRepositories")]
	public class ExchangeRateRepositoryDbm : IEntityDBM<ExchangeRateRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		[Column("Type")]
		public int Type { get; set; }

		public ExchangeRateRepositoryDbm() { }

		public ExchangeRateRepositoryDbm(ExchangeRateRepository repository)
		{
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		public Task<ExchangeRateRepository> Resolve()
		{
			return Task.Factory.StartNew<ExchangeRateRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalExchangeRateRepository(Id);
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexExchangeRateRepository(Id);
					case DB_TYPE_BTCE_REPOSITORY: return new BtceExchangeRateRepository(Id);
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorExchangeRateRepository(Id);
					default: throw new NotSupportedException();
				}
			});
		}
	}
}

