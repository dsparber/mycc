using System.Threading.Tasks;
using data.database.interfaces;
using SQLite;
using Xamarin.Forms;

namespace data.database.helper
{
	public abstract class AbstractDatabase
	{
		SQLiteAsyncConnection connection;

		protected SQLiteAsyncConnection ConnectionWithoutCreate { get { return connection; } }

		public async Task<SQLiteAsyncConnection> Connection()
		{
			await Create();
			return connection;
		}

		protected AbstractDatabase()
		{
			connection = DependencyService.Get<ISQLiteConnection>().GetConnection();
		}

		protected abstract Task<CreateTablesResult> Create();
	}
}

