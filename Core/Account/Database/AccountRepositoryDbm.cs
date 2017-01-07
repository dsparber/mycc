using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using SQLite;

namespace MyCryptos.Core.Account.Database
{
	[Table("AccountRepositories")]
	public class AccountRepositoryDbm : IEntityDBM<AccountRepository, int>
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BITTREX_REPOSITORY = 2;
		public const int DB_TYPE_BLOCK_EXPERTS_REPOSITORY = 3;
		public const int DB_TYPE_BLOCKCHAIN_REPOSITORY = 4;
		public const int DB_TYPE_ETHEREUM_REPOSITORY = 5;
		public const int DB_TYPE_CRYPTOID_REPOSITORY = 6;
		public const int DB_TYPE_BLOCKCHAIN_XPUB_REPOSITORY = 7;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		[Column("Name")]
		public string Name { get; set; }

		[Column("Type")]
		public int Type { get; set; }

		/// <summary>
		/// Json formated additional data for the account. E.g. Api-Key
		/// </summary>
		/// <value>Json data</value>
		public string Data { get; set; }

		public AccountRepositoryDbm() { }

		public AccountRepositoryDbm(AccountRepository repository)
		{
			Name = repository.Name;
			Data = repository.Data;
			Type = repository.RepositoryTypeId;
			Id = repository.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is AccountRepositoryDbm)
			{
				return Id == ((AccountRepositoryDbm)obj).Id;
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
					case DB_TYPE_LOCAL_REPOSITORY: return new LocalAccountRepository(Id, Name);
					case DB_TYPE_BITTREX_REPOSITORY: return new BittrexAccountRepository(Id, Name, Data);
					case DB_TYPE_BLOCK_EXPERTS_REPOSITORY: return new BlockExpertsAccountRepository(Id, Name, Data);
					case DB_TYPE_BLOCKCHAIN_REPOSITORY: return new BlockchainAccountRepository(Id, Name, Data);
					case DB_TYPE_ETHEREUM_REPOSITORY: return new EthereumAccountRepository(Id, Name, Data);
					case DB_TYPE_CRYPTOID_REPOSITORY: return new CryptoIdAccountRepository(Id, Name, Data);
					case DB_TYPE_BLOCKCHAIN_XPUB_REPOSITORY: return new BlockchainXpubAccountRepository(Id, Name, Data);
					default: throw new NotSupportedException();
				}
			});
		}
	}
}