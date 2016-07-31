using System.Threading.Tasks;
using SQLite;

namespace data.database.helper
{
	public class DatabaseHelper
	{
		DatabaseHelper(){}

		public static async Task InsertOrUpdate(SQLiteAsyncConnection database, object dbObject)
		{
			var rowsAffected = await database.UpdateAsync(dbObject);
			if (rowsAffected == 0)
			{
				await database.InsertAsync(dbObject);
			}
		}
	}
}

