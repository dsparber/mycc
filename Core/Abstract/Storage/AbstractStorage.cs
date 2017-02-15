using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Repositories;
using MyCC.Core.Settings;

namespace MyCC.Core.Abstract.Storage
{
    public abstract class AbstractStorage<T, TV> where TV : AbstractRepository where T : IEntityDbm<TV, int>
    {
        public List<TV> Repositories { get; private set; }

        private readonly Task _onCreationTask;

        protected AbstractStorage(AbstractDatabase<T, TV, int> database)
        {
            Database = database;
            Repositories = new List<TV>();
            _onCreationTask = OnCreation();
        }

        private AbstractDatabase<T, TV, int> Database { get; set; }

        private async Task OnCreation()
        {
            Repositories.AddRange(await Database.GetAll());
            if (ApplicationSettings.FirstLaunch)
            {
                await OnFirstLaunch();
            }
        }
        protected virtual Task OnFirstLaunch() { return Task.Factory.StartNew(() => { }); }


        protected async Task Add(TV repository)
        {
            repository = await Database.Insert(repository);
            Repositories.Add(repository);
        }

        public virtual async Task Remove(TV repository)
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

        private List<TA> RepositoriesOfType<TA>() where TA : TV
        {
            return Repositories.OfType<TA>().ToList();
        }

        public List<TV> RepositoriesOfType(Type type)
        {
            return Repositories.FindAll(r => r.GetType() == type);
        }

        public TA RepositoryOfType<TA>() where TA : TV
        {
            return RepositoriesOfType<TA>().FirstOrDefault();
        }

        public TV RepositoryOfType(Type type)
        {
            return RepositoriesOfType(type).FirstOrDefault();
        }

        protected virtual Task BeforeFastFetching()
        {
            return Task.Factory.StartNew(() => { });
        }

        public async Task FetchOnline(Action<double> progressCallback = null)
        {
            await _onCreationTask;
            var i = .0;
            await Task.WhenAll(Repositories.Select(async x =>
            {
                await x.FetchOnline();
                i += 1;
                progressCallback?.Invoke(i / Repositories.Count);
            }));
        }

        public async Task LoadFromDatabase()
        {
            await _onCreationTask;
            await BeforeFastFetching();
            await Task.WhenAll(Repositories.Select(x => x.LoadFromDatabase()));
        }
    }
}