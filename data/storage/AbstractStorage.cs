using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.repositories.general;
using data.database.interfaces;
using data.database.helper;
using data.settings;

namespace data.storage
{
	public abstract class AbstractStorage<T, R, D, V> where R : AbstractRepository<D, V> where D : IEntityRepositoryIdDBM<V>
	{
		private IEnumerable<R> repositories;

		public abstract AbstractRepositoryDatabase<T> GetDatabase();

		protected abstract R Resolve(T obj);

		protected virtual Task OnFirstLaunch() { return Task.Factory.StartNew(() => { }); }

		Task initialisation;

		async Task initialise()
		{
			if (ApplicationSettings.FirstLaunch)
			{
				if (initialisation == null)
				{
					initialisation = OnFirstLaunch();
				}
				if (!initialisation.IsCompleted)
				{
					await initialisation;
				}
			}
		}

		public async Task<List<R>> Repositories()
		{
			if (repositories == null)
			{
				await initialise();
				var repos = await GetDatabase().GetRepositories();
				repositories = repos.Select(r => Resolve(r)).ToList();
			}
			return repositories.ToList();
		}

		public async Task<List<R>> RepositoriesOfType<A>()
		{
			return (await Repositories()).FindAll(r => r is A);
		}

		public async Task<List<V>> AllElements()
		{
			return (await Repositories()).SelectMany(r => r.Elements).ToList();
		}

		public async Task<List<V>> AllOfType<A>()
		{
			return (await AllElements()).FindAll(e => e is A);
		}

		public async Task Fetch()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.Fetch()));
		}

		public async Task FetchFast()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchFast()));
		}
	}
}