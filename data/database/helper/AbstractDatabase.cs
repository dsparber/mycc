using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.interfaces;
using SQLite;
using Xamarin.Forms;

namespace data.database.helper
{
	public abstract class AbstractDatabase<T, V> where T : IDBM<V>
	{
		public SQLiteAsyncConnection Connection { get; private set; }

		protected AbstractDatabase()
		{
			Connection = DependencyService.Get<ISQLiteConnection>().GetConnection();
		}

		public abstract Task<CreateTablesResult> Create();

		public abstract Task<IEnumerable<T>> GetAllDbObjects();

		public abstract Task Write(IEnumerable<V> data);

		public virtual async Task<IEnumerable<V>> GetAll()
		{
			return await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()));
		}

		public async Task<T> GetDbObject(int id)
		{
			return (await GetAllDbObjects()).Single(o => o.Id == id);
		}

		public async Task<V> Get(int id)
		{
			return await (await GetDbObject(id)).Resolve();
		}
	}
}

