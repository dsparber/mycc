using data.repositories.account;
using SQLite;

namespace data.database.models
{
	[Table("AccountRepositories")]
	public class AccountRepositoryDBM
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BITTREX_REPOSITORY = 2;


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
	}
}

