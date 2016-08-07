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

		public abstract AbstractRepositoryDatabase<T> GetDatabase();

		protected abstract R Resolve(T obj);

		protected abstract AbstractStorage<T, R, D, V> CreateInstance();

		protected virtual Task OnFirstLaunch() { return Task.Factory.StartNew(() => { }); }

		Task initialisation;

		async Task initialise()
		{
			if (Settings.Get(Settings.KEY_FIRST_LAUNCH, false))
			{
				if (initialisation == null)
				{
					initialisation = OnFirstLaunch();
				}
				if (!initialisation.IsCompleted)
				{
					await initialisation;
				}
				Settings.Set(Settings.KEY_FIRST_LAUNCH, true);
			}
		}

		public async Task<List<R>> Repositories()
		{
			await initialise();
			var repos = await GetDatabase().GetRepositories();
			return repos.Select(r => Resolve(r)).ToList();
		}

		public async Task<List<V>> AllElements()
		{
			return (await Repositories()).SelectMany(v => v.Elements).ToList();
		}

		public async Task Fetch()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.Fetch()));
		}

		public async Task FetchFast()
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchFast()));
		}

		AbstractStorage<T, R, D, V> instance { get; set; }

		public AbstractStorage<T, R, D, V> Instance
		{
			get
			{
				if (instance == null)
				{
					instance = CreateInstance();
				}
				return instance;
			}
		}
	}
}

