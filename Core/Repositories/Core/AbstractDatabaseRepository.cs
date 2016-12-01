using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Repositories.Core
{
    public abstract class AbstractDatabaseRepository<T, V, IdType> : AbstractRepository where T : IEntityRepositoryIdDBM<V, IdType> where V : PersistableRepositoryElement<IdType>
    {
        List<V> elements;

        public IEnumerable<V> Elements
        {
            get { return elements.FindAll(e => true); }
        }

        public DateTime LastFastFetch { get; protected set; }
        public DateTime LastFetch { get; protected set; }

        readonly AbstractDatabase<T, V, IdType> Database;

        protected AbstractDatabaseRepository(int repositoryId, string name, AbstractDatabase<T, V, IdType> database) : base(repositoryId, name)
        {
            elements = new List<V>();
            Database = database;
        }

        protected virtual Func<V, bool> DatabaseFilter
        {
            get { return v => v.RepositoryId == Id; }
        }

        protected virtual async Task<bool> FetchFromDatabase()
        {
            try
            {
                elements = new List<V>((await Database.GetAll()).Where(DatabaseFilter));
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

        public async Task AddOrUpdate(V element)
        {
            elements.Remove(element);
            await Database.InsertOrUpdate(element);
            elements.Add(element);
        }

        public async Task Add(IEnumerable<V> newElements)
        {
            newElements = await Database.Insert(newElements);
            elements.AddRange(newElements);
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