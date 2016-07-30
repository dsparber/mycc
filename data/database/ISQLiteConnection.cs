using SQLite;

namespace data.database
{
	public interface ISQLiteConnection
	{
		SQLiteAsyncConnection GetConnection();
	}
}

