using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Models;
using SQLite;
using Xamarin.Forms;

namespace MyCC.Core.Abstract.Database
{
    public abstract class AbstractDatabase<T, TV, TIdType> where T : IEntityDbm<TV, TIdType> where TV : IPersistable<TIdType>
    {
        private SQLiteAsyncConnection _connection;

        private Task _initialisation;

        private async Task Initialise()
        {
            if (_initialisation == null)
            {
                _initialisation = Create(_connection);
            }
            if (!_initialisation.IsCompleted)
            {
                await _initialisation;
            }
        }

        private async Task<SQLiteAsyncConnection> GetConnection()
        {
            await Initialise();
            return _connection;
        }

        public Task<SQLiteAsyncConnection> Connection => GetConnection();

        protected abstract Task Create(SQLiteAsyncConnection connection);

        protected AbstractDatabase()
        {
            _connection = DependencyService.Get<ISqLiteConnection>().GetConnection();
        }


        public async Task<TV> Insert(TV element)
        {
            var dbElement = Resolve(element);
            await (await Connection).InsertAsync(dbElement);
            element.Id = dbElement.Id;

            return element;
        }
        public async Task<TV> InsertOrUpdate(TV element)
        {
            var dbElement = Resolve(element);
            await (await Connection).InsertOrReplaceAsync(dbElement);
            element.Id = dbElement.Id;

            return element;
        }
        public async Task<IEnumerable<TV>> Insert(IEnumerable<TV> elemets)
        {
            var dbElements = elemets.Distinct().Select(Resolve);
            await (await Connection).InsertAllAsync(dbElements);
            return await Task.WhenAll(dbElements.Select(e => e.Resolve()));
        }

        public async Task<TV> Update(TV element)
        {
            return await Update(element, element);
        }

        public async Task<TV> Update(TV oldElement, TV newElement)
        {
            if (EqualityComparer<TIdType>.Default.Equals(oldElement.Id, newElement.Id) && !EqualityComparer<TIdType>.Default.Equals(default(TIdType), newElement.Id))
            {
                await (await Connection).UpdateAsync(Resolve(newElement));
                return newElement;
            }
            // else
            await Delete(oldElement);
            return await Insert(newElement);
        }

        public async Task<IEnumerable<TV>> Update(IEnumerable<TV> elemets)
        {
            var dbElements = elemets.Select(Resolve);
            await (await Connection).UpdateAllAsync(elemets);
            return await Task.WhenAll(dbElements.Select(e => e.Resolve()));
        }

        public async Task Delete(TV element)
        {
            await (await Connection).DeleteAsync(Resolve(element));
        }

        public abstract Task<IEnumerable<T>> GetAllDbObjects();
        public async Task<IEnumerable<TV>> GetAll()
        {
            return await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()));
        }

        public async Task<IEnumerable<TV>> Get(Func<T, bool> predicate)
        {
            return await Task.WhenAll((await GetAllDbObjects()).Where(predicate).Select(o => o.Resolve()));
        }

        public async Task<IEnumerable<TV>> Get(Func<TV, bool> predicate)
        {
            return (await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()))).Where(predicate);
        }

        public abstract Task<T> GetDbObject(TIdType id);
        public async Task<TV> Get(TIdType id)
        {
            var element = await GetDbObject(id);
            return await ResolveReverse(element);
        }

        public async Task<TV> ResolveReverse(T element)
        {
            if (!EqualityComparer<T>.Default.Equals(element, default(T)))
            {
                return await element.Resolve();
            }
            return default(TV);
        }

        protected abstract T Resolve(TV element);
    }
}

