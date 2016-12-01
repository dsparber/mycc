using SQLite;

namespace data.database.interfaces
{
	public interface ISQLiteConnection
	{
		SQLiteAsyncConnection GetConnection();
	}
}

