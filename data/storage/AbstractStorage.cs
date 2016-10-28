using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.repositories.general;
using data.settings;
using System;
using data.database.interfaces;
using MyCryptos.data.database.helper;

namespace data.storage
{
	public abstract class AbstractStorage<T, V> where V : AbstractRepository where T : IEntityDBM<V>
	{
		protected List<V> repositories;

		protected AbstractStorage(AbstractDatabase<T, V> database)
		{
			Database = database;
		}

		AbstractDatabase<T, V> Database { get; set; }

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

		public async Task<List<V>> Repositories()
		{
			if (repositories == null)
			{
				await initialise();
				repositories = (await Database.GetAll()).ToList();
			}
			return repositories;
		}

		public virtual async Task Add(V repository)
		{
			await Repositories();
			repository = await Database.Insert(repository);
			repositories.Add(repository);
		}

		public virtual async Task Remove(V repository)
		{
			await Repositories();
			await Database.Delete(repository);
			repositories.Remove(repository);
		}

		public virtual async Task Update(V repository)
		{
			await Repositories();
			repositories.Remove(repository);
			repository = await Database.Update(repository);
			repositories.Add(repository);
		}

		public async Task<List<A>> RepositoriesOfType<A>() where A : V
		{
			return (await Repositories()).OfType<A>().ToList();
		}

		public async Task<List<V>> RepositoriesOfType(Type type)
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