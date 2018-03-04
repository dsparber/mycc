using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Models;
using MyCC.Core.Database;
using SQLite;

namespace MyCC.Core.Abstract.Database
{
    public abstract class AbstractDatabase<T, TV, TIdType> where T : IEntityDbm<TV, TIdType> where TV : IPersistable<TIdType>
    {
        private readonly SQLiteAsyncConnection _connection;

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

        protected Task<SQLiteAsyncConnection> Connection => GetConnection();

        protected abstract Task Create(SQLiteAsyncConnection connection);

        protected AbstractDatabase()
        {
            _connection = DatabaseUtil.OldConnection;
        }


        public async Task<TV> Insert(TV element)
        {
            var dbElement = Resolve(element);
            await (await Connection).InsertAsync(dbElement);
            element.Id = dbElement.Id;

            return element;
        }

        public async Task<TV> Update(TV element)
        {
            if (!EqualityComparer<TIdType>.Default.Equals(default(TIdType), element.Id))
            {
                await (await Connection).UpdateAsync(Resolve(element));
                return element;
            }
            // else
            await Delete(element);
            return await Insert(element);
        }

        public async Task Delete(TV element)
        {
            await (await Connection).DeleteAsync(Resolve(element));
        }

        protected abstract Task<IEnumerable<T>> GetAllDbObjects();
        public async Task<IEnumerable<TV>> GetAll()
        {
            return (await Task.WhenAll((await GetAllDbObjects()).Select(o => o.Resolve()))).Where(o => o != null);
        }

        protected abstract Task<T> GetDbObject(TIdType id);
        public async Task<TV> Get(TIdType id)
        {
            var element = await GetDbObject(id);
            return await ResolveReverse(element);
        }

        private static async Task<TV> ResolveReverse(T element)
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

