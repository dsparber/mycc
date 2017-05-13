using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Abstract.Repositories
{
    public abstract class AbstractDatabaseRepository<TDatabaseModel, TModel, TId> : AbstractRepository where TDatabaseModel : IEntityRepositoryIdDbm<TModel, TId> where TModel : IPersistableWithParent<TId>
    {
        private List<TModel> _elements;

        public IEnumerable<TModel> Elements => _elements;

        public void ClearElements() => _elements.Clear();

        public DateTime LastFastFetch { get; protected set; }
        public DateTime LastFetch { get; protected set; }

        private readonly AbstractDatabase<TDatabaseModel, TModel, TId> _database;

        protected AbstractDatabaseRepository(int id, AbstractDatabase<TDatabaseModel, TModel, TId> database) : base(id)
        {
            _elements = new List<TModel>();
            _database = database;
        }

        protected virtual Func<TModel, bool> DatabaseFilter
        {
            get { return v => v != null && v.ParentId == Id; }
        }

        protected async Task<bool> FetchFromDatabase()
        {
            try
            {
                _elements = new List<TModel>((await _database.GetAll()).Where(DatabaseFilter));
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
            await _database.DeleteAll();
            _elements.Clear();
        }

        public async Task Remove(TModel element)
        {
            await _database.Delete(element);
            _elements.Remove(element);
        }

        public async Task Add(TModel element)
        {
            element = await _database.Insert(element);
            _elements.Add(element);
        }

        public async Task AddOrUpdate(TModel element)
        {
            _elements.RemoveAll(e => Equals(e, element));
            await _database.InsertOrUpdate(element);
            _elements.Add(element);
        }

        public async Task Add(IEnumerable<TModel> newElements)
        {
            newElements = await _database.Insert(newElements);
            _elements.AddRange(newElements);
        }

        public async Task Update(IEnumerable<TModel> updateElements)
        {
            var enumerable = updateElements as IList<TModel> ?? updateElements.ToList();
            _elements.RemoveAll(enumerable.Contains);
            updateElements = await _database.Update(enumerable);
            _elements.AddRange(updateElements);
        }

        public async Task Update(TModel element)
        {
            await Update(element, element);
        }

        public async Task Update(TModel oldElement, TModel newElement)
        {
            _elements.Remove(oldElement);
            newElement = await _database.Update(oldElement, newElement);
            _elements.Add(newElement);
        }
    }
}