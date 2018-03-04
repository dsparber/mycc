using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using MyCC.Core.Helpers;

namespace MyCC.Core.Abstract.Repositories
{
    public abstract class AbstractDatabaseRepository<TDatabaseModel, TModel, TId> : AbstractRepository where TDatabaseModel : IEntityRepositoryIdDbm<TModel, TId> where TModel : IPersistableWithParent<TId>
    {
        private List<TModel> _elements;

        public IEnumerable<TModel> Elements => _elements.ToArray();

        public DateTime LastFetch { get; set; }

        private readonly AbstractDatabase<TDatabaseModel, TModel, TId> _database;

        protected AbstractDatabaseRepository(int id, AbstractDatabase<TDatabaseModel, TModel, TId> database) : base(id)
        {
            _elements = new List<TModel>();
            _database = database;
        }

        protected async Task<bool> FetchFromDatabase()
        {
            try
            {
                _elements = new List<TModel>((await _database.GetAll()).Where(v => v != null && v.ParentId == Id));
                return true;
            }
            catch (Exception e)
            {
                e.LogError();
                return false;
            }
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

        public async Task Update(TModel element)
        {
            _elements.Remove(element);
            _elements.Add(await _database.Update(element));
        }
    }
}