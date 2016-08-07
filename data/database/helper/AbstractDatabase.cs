using System.Threading.Tasks;
using data.database.interfaces;
using SQLite;
using Xamarin.Forms;

namespace data.database.helper
{
	public abstract class AbstractDatabase
	{
		SQLiteAsyncConnection connection;

		Task initialisation;

		async Task initialise()
		{
			if (initialisation == null)
			{
				initialisation = Create();
			}
			if (!initialisation.IsCompleted)
			{
				await initialisation;
			}
		}

		protected SQLiteAsyncConnection ConnectionWithoutCreate { get { return connection; } }

		public async Task<SQLiteAsyncConnection> Connection()
		{
			await initialise();
			return connection;
		}

		protected AbstractDatabase()
		{
			connection = DependencyService.Get<ISQLiteConnection>().GetConnection();
		}

		protected abstract Task<CreateTablesResult> Create();
	}
}

