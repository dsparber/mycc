using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Currency.Repositories;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
	[Table("CurrencyRepositories")]
	public class CurrencyRepositoryDbm : IEntityDBM<CurrencyRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;
		public const int DB_TYPE_BLOCK_EXPERTS_REPOSITORY = 5;
		public const int DB_TYPE_CRYPTOID_REPOSITORY = 6;


		public CurrencyRepositoryDbm() { }

		public CurrencyRepositoryDbm(CurrencyRepository repository)
		{
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		[Column("Type")]
		public int Type { get; set; }

		public Task<CurrencyRepository> Resolve()
		{
			return Task.Factory.StartNew<CurrencyRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalCurrencyRepository(Id);
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexCurrencyRepository(Id);
					case DB_TYPE_BTCE_REPOSITORY: return new BtceCurrencyRepository(Id);
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorCurrencyRepository(Id);
					case DB_TYPE_BLOCK_EXPERTS_REPOSITORY: return new BlockExpertsCurrencyRepository(Id);
					case DB_TYPE_CRYPTOID_REPOSITORY: return new CryptoIdCurrencyRepository(Id);
					default: throw new NotSupportedException();
				}
			});
		}
	}
}

