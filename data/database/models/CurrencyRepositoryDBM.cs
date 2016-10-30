using System;
using System.Threading.Tasks;
using data.database.interfaces;
using data.repositories.currency;
using SQLite;
namespace data.database.models
{
	[Table("CurrencyRepositories")]
	public class CurrencyRepositoryDBM : IEntityDBM<CurrencyRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;
		public const int DB_TYPE_BITTREX_REPOSITORY = 3;
		public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;
		public const int DB_TYPE_BLOCK_EXPERTS_REPOSITORY = 5;


		public CurrencyRepositoryDBM() { }

		public CurrencyRepositoryDBM(CurrencyRepository repository)
		{
			Name = repository.Name;
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set; }

		public Task<CurrencyRepository> Resolve()
		{
			return Task.Factory.StartNew<CurrencyRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalCurrencyRepository(Name) { Id = Id };
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexCurrencyRepository(Name) { Id = Id };
					case DB_TYPE_BTCE_REPOSITORY: return new BtceCurrencyRepository(Name) { Id = Id };
					case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorCurrencyRepository(Name) { Id = Id };
					case DB_TYPE_BLOCK_EXPERTS_REPOSITORY: return new BlockExpertsCurrencyRepository(Name) { Id = Id };
					default: throw new NotImplementedException();
				}
			});
		}
	}
}

