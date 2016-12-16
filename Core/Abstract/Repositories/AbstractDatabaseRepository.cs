using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Abstract.Models;

namespace MyCryptos.Core.Abstract.Repositories
{
	public abstract class AbstractDatabaseRepository<TDatabaseModel, TModel, TId> : AbstractRepository where TDatabaseModel : IEntityRepositoryIdDBM<TModel, TId> where TModel : IPersistableWithParent<TId>
	{
		List<TModel> elements;

		public IEnumerable<TModel> Elements
		{
			get { return elements.FindAll(e => true); }
		}

		public int ElementsCount => Elements.ToList().Count;

		public DateTime LastFastFetch { get; protected set; }
		public DateTime LastFetch { get; protected set; }

		readonly AbstractDatabase<TDatabaseModel, TModel, TId> Database;

		protected AbstractDatabaseRepository(int id, AbstractDatabase<TDatabaseModel, TModel, TId> database) : base(id)
		{
			elements = new List<TModel>();
			Database = database;
		}

		protected virtual Func<TModel, bool> DatabaseFilter
		{
			get { return v => v.ParentId == Id; }
		}

		protected virtual async Task<bool> FetchFromDatabase()
		{
			try
			{
				elements = new List<TModel>((await Database.GetAll()).Where(DatabaseFilter));
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Error Message:\n{e.Message}\nData:\n{e.Data}\nStack trace:\n{e.StackTrace}");
				return false;
			}
		}

		public async Task RemoveAll()
		{
			await Task.WhenAll(Elements.Select(e => Database.Delete(e)));
			elements.RemoveAll(e => true);
		}

		public async Task Remove(TModel element)
		{
			await Database.Delete(element);
			elements.Remove(element);
		}

		public async Task Add(TModel element)
		{
			element = await Database.Insert(element);
			elements.Add(element);
		}

		public async Task AddOrUpdate(TModel element)
		{
			elements.RemoveAll(e => Equals(e, element));
			await Database.InsertOrUpdate(element);
			elements.Add(element);
		}

		public async Task Add(IEnumerable<TModel> newElements)
		{
			newElements = await Database.Insert(newElements);
			elements.AddRange(newElements);
		}

		public async Task Update(IEnumerable<TModel> updateElements)
		{
			elements.RemoveAll(updateElements.Contains);
			updateElements = await Database.Update(updateElements);
			elements.AddRange(updateElements);
		}

		public async Task Update(TModel element)
		{
			await Update(element, element);
		}

		public async Task Update(TModel oldElement, TModel newElement)
		{
			elements.Remove(oldElement);
			newElement = await Database.Update(oldElement, newElement);
			elements.Add(newElement);
		}
	}
}