using SQLite;

namespace data.database.models
{
	[Table("Repositories")]
	public class RepositoryDBM
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}

