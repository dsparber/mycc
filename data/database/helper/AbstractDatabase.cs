using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;
using SQLite;
using Xamarin.Forms;

namespace MyCryptos.data.database.helper
{
	public abstract class AbstractDatabase<T, V> where T : IEntityDBM<V> where V : Persistable
	{
		SQLiteAsyncConnection connection;

		Task initialisation;

		async Task initialise()
		{
			if (initialisation == null)
			{
				initialisation = Create(connection);
			}
			if (!initialisation.IsCompleted)
			{
				await initialisation;
			}
		}

		async Task<SQLiteAsyncConnection> getConnection()
		{
			await initialise();
			return connection;
		}

		public Task<SQLiteAsyncConnection> Connection
		{
			get { return getConnection(); }
		}

		protected abstract Task Create(SQLiteAsyncConnection connection);

		protected AbstractDatabase()
		{
			connection = DependencyService.Get<ISQLiteConnection>().GetConnection();
		}


		public async Task<V> Insert(V element)
		{
			var dbElement = Resolve(element);
			await (await Connection).InsertAsync(dbElement);
			element.Id = dbElement.Id;
			return element;
		}
		public async Task<IEnumerable<V>> Insert(IEnumerable<V> elemets)
		{
			var dbElements = elemets.Select(e => Resolve(e));
			await (await Connection).InsertAllAsync(dbElements);
			return await Task.WhenAll(dbElements.Select(e => e.Resolve()));
		}

		public async Task<V> Update(V element)
		{
			return await Update(element, element);
		}

		public async Task<V> Update(V oldElement, V newElement)
		{
			if (newElement.Id.HasValue && oldElement.Id == newElement.Id)
			{
				await (await Connection).UpdateAsync(Resolve(newElement));
				return newElement;
			}
			// else
			await Delete(oldElement);
			return await Insert(newElement);
		}

		public async Task<IEnumerable<V>> Update(IEnumerable<V> elemets)
		{
			var dbElements = elemets.Select(e => Resolve(e));
			await (await Connection).UpdateAllAsync(elemets);
			return await Task.WhenAll(dbElements.Select(e => e.Resolve()));
		}

		public async Task Delete(V element)
		{
			await (await Connection).DeleteAsync(Resolve(element));
		}

		public abstract Task<IEnumerable<T>> GetAllDbObjects();
		public async Task<IEnumerable<V>> GetAll()
		{
			return await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()));
		}

		public abstract Task<T> GetDbObject(int id);
		public async Task<V> Get(int id)
		{
			var element = await GetDbObject(id);
			if (!EqualityComparer<T>.Default.Equals(element, default(T)))
			{
				return await element.Resolve();
			}
			return default(V);
		}

		protected abstract T Resolve(V element);
	}
}

