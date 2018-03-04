using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Repositories;

namespace MyCC.Core.Abstract.Storage
{
    public abstract class AbstractStorage<T, TV> where TV : AbstractRepository where T : IEntityDbm<TV, int>
    {
        public List<TV> Repositories { get; }

        private readonly Task _onCreationTask;

        protected AbstractStorage(AbstractDatabase<T, TV, int> database)
        {
            Database = database;
            Repositories = new List<TV>();
            _onCreationTask = OnCreation();
        }

        private AbstractDatabase<T, TV, int> Database { get; }

        private async Task OnCreation()
        {
            Repositories.AddRange(await Database.GetAll());

            await AfterLoadingRepositories();
        }
        protected virtual Task AfterLoadingRepositories() { return Task.Factory.StartNew(() => { }); }


        public async Task Add(TV repository)
        {
            repository = await Database.Insert(repository);
            Repositories.Add(repository);
        }

        public async Task Remove(TV repository)
        {
            await Database.Delete(repository);
            Repositories.Remove(repository);
        }

        public async Task Update(TV repository)
        {
            Repositories.Remove(repository);
            repository = await Database.Update(repository);
            Repositories.Add(repository);
        }


        protected List<TV> RepositoriesOfType(Type type)
        {
            return Repositories.FindAll(r => r.GetType() == type);
        }

        public async Task FetchOnline(Action<double> progressCallback = null)
        {
            await _onCreationTask;
            var i = .0;

            foreach (var x in Repositories.ToArray())
            {
                await x.FetchOnline();
                i += 1;
                progressCallback?.Invoke(i / Repositories.Count);
            }
        }


        public async Task LoadFromDatabase()
        {
            await _onCreationTask;
            await Task.WhenAll(Repositories.Select(x => x.LoadFromDatabase()));
        }
    }
}