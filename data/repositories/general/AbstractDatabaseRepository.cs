using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.data.database.helper;
using MyCryptos.models;

namespace data.repositories.general
{
	public abstract class AbstractDatabaseRepository<T, V> : AbstractRepository where T : IEntityRepositoryIdDBM<V> where V : PersistableRepositoryElement
	{
		List<V> elements;

		public IEnumerable<V> Elements
		{
			get { return elements.FindAll(e => true); }
		}

		public DateTime LastFastFetch { get; protected set; }
		public DateTime LastFetch { get; protected set; }

		readonly AbstractDatabase<T, V> Database;

		protected AbstractDatabaseRepository(int repositoryId, string name, AbstractDatabase<T, V> database) : base(repositoryId, name)
		{
			elements = new List<V>();
			Database = database;
		}

		protected async Task<bool> FetchFromDatabase()
		{
			try
			{
				elements = new List<V>((await Database.GetAll()).Where(v => v.RepositoryId == Id));
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}

		public async Task RemoveAll()
		{
			await Task.WhenAll(Elements.Select(e => Database.Delete(e)));
			elements.RemoveAll(e => true);
		}

		public async Task Remove(V element)
		{
			await Database.Delete(element);
			elements.Remove(element);
		}

		public async Task Add(V element)
		{
			element = await Database.Insert(element);
			elements.Add(element);
		}

		public async Task Add(IEnumerable<V> newElements)
		{
			newElements = await Database.Insert(newElements);
			elements.AddRange(elements);
		}

		public async Task Update(IEnumerable<V> updateElements)
		{
			elements.RemoveAll(updateElements.Contains);
			updateElements = await Database.Update(updateElements);
			elements.AddRange(updateElements);
		}

		public async Task Update(V element)
		{
			await Update(element, element);
		}

		public async Task Update(V oldElement, V newElement)
		{
			elements.Remove(oldElement);
			newElement = await Database.Update(oldElement, newElement);
			elements.Add(newElement);
		}
	}
}