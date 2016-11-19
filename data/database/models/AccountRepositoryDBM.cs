using System;
using System.Threading.Tasks;
using data.database.interfaces;
using data.repositories.account;
using SQLite;
using MyCryptos.data.repositories.account;

namespace data.database.models
{
	[Table("AccountRepositories")]
	public class AccountRepositoryDBM : IEntityDBM<AccountRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BITTREX_REPOSITORY = 2;
		public const int DB_TYPE_BLOCK_EXPERTS_REPOSITORY = 3;
		public const int DB_TYPE_BLOCKCHAIN_REPOSITORY = 4;
        public const int DB_TYPE_ETHEREUM_REPOSITORY = 5;
        public const int DB_TYPE_CRYPTOID_REPOSITORY = 6;

        [PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set; }

		/// <summary>
		/// Json formated additional data for the account. E.g. Api-Key
		/// </summary>
		/// <value>Json data</value>
		public string Data { get; set; }

		public AccountRepositoryDBM() { }

		public AccountRepositoryDBM(AccountRepository repository)
		{
			Name = repository.Name;
			Data = repository.Data;
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is AccountRepositoryDBM)
			{
				return Id == ((AccountRepositoryDBM)obj).Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public Task<AccountRepository> Resolve()
		{
			return Task.Factory.StartNew<AccountRepository>(() =>
			{
				switch (Type)
				{
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalAccountRepository(Name) { Id = Id };
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexAccountRepository(Name, Data) { Id = Id };
					case DB_TYPE_BLOCK_EXPERTS_REPOSITORY: return new BlockExpertsAccountRepository(Name, Data) { Id = Id };
					case DB_TYPE_BLOCKCHAIN_REPOSITORY: return new BlockchainAccountRepository(Name, Data) { Id = Id };
					case DB_TYPE_ETHEREUM_REPOSITORY: return new EthereumAccountRepository(Name, Data) { Id = Id };
					case DB_TYPE_CRYPTOID_REPOSITORY: return new CryptoIdAccountRepository(Name, Data) { Id = Id };
                    default: throw new NotSupportedException();
				}
			});
		}
	}
}