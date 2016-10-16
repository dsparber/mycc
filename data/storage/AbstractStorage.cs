using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.repositories.general;
using data.database.helper;
using data.settings;
using System;

namespace data.storage
{
	public abstract class AbstractStorage<T, R, V> where R : AbstractRepository<V>
	{
		protected IEnumerable<R> repositories;

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

		public async Task AddRepository(T repository)
		{
			await Repositories();
			await GetDatabase().AddRepository(repository);
			var repos = await GetDatabase().GetRepositories();
			repositories = repos.Select(r => Resolve(r)).ToList();
		}

		public virtual async Task Remove(T repository)
		{
			await Repositories();

			await GetDatabase().Remove(repository);
			var repos = await GetDatabase().GetRepositories();
			repositories = repos.Select(r => Resolve(r)).ToList();
		}

		public async Task<List<R>> RepositoriesOfType<A>()
		{
			return (await Repositories()).FindAll(r => r is A);
		}

		public async Task<List<R>> RepositoriesOfType(Type type)
		{
			return (await Repositories()).FindAll(r => r.GetType() == type);
		}

		public virtual async Task Fetch()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.Fetch()));
		}

		public virtual async Task FetchFast()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchFast()));
		}
	}
}