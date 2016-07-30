using SQLite;

namespace data.database.models
{
	[Table("Repositories")]
	public class AccountRepositoryDBM
	{
		public static const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public static const int DB_TYPE_BITTREX_REPOSITORY = 2;


		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set;}
	}
}

