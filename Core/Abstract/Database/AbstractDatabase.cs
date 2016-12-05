using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Models;
using SQLite;
using Xamarin.Forms;

namespace MyCryptos.Core.Abstract.Database
{
    public abstract class AbstractDatabase<T, V, IdType> where T : IEntityDBM<V, IdType> where V : Persistable<IdType>
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
        public async Task<V> InsertOrUpdate(V element)
        {
            var dbElement = Resolve(element);
            await (await Connection).InsertOrReplaceAsync(dbElement);
            element.Id = dbElement.Id;

            return element;
        }
        public async Task<IEnumerable<V>> Insert(IEnumerable<V> elemets)
        {
            var dbElements = elemets.Distinct().Select(e => Resolve(e));
            await (await Connection).InsertAllAsync(dbElements);
            return await Task.WhenAll(dbElements.Select(e => e.Resolve()));
        }

        public async Task<V> Update(V element)
        {
            return await Update(element, element);
        }

        public async Task<V> Update(V oldElement, V newElement)
        {
            if (EqualityComparer<IdType>.Default.Equals(oldElement.Id, newElement.Id) && !EqualityComparer<IdType>.Default.Equals(default(IdType), newElement.Id))
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

        public async Task<IEnumerable<V>> Get(Func<T, bool> predicate)
        {
            return await Task.WhenAll((await GetAllDbObjects()).Where(predicate).Select(o => o.Resolve()));
        }

        public async Task<IEnumerable<V>> Get(Func<V, bool> predicate)
        {
            return (await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()))).Where(predicate);
        }

        public abstract Task<T> GetDbObject(IdType id);
        public async Task<V> Get(IdType id)
        {
            var element = await GetDbObject(id);
            return await ResolveReverse(element);
        }

        public async Task<V> ResolveReverse(T element)
        {
            if (!EqualityComparer<T>.Default.Equals(element, default(T)))
            {
                return await element.Resolve();
            }
            return default(V);
        }

        protected abstract T Resolve(V element);
    }
}

