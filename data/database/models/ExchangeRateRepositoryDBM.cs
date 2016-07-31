using SQLite;

namespace data.database.models
{
	public class ExchangeRateRepositoryDBM
	{
		public const int DB_TYPE_LOCAL_REPOSITORY = 1;
		public const int DB_TYPE_BTCE_REPOSITORY = 2;

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public int Type { get; set; }
	}
}

