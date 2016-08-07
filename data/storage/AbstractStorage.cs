using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.repositories.general;
using data.database.interfaces;
using data.database.helper;

namespace data.storage
{
	public abstract class AbstractStorage<T, R, D, V> where R : AbstractRepository<D, V> where D : IEntityRepositoryIdDBM<V>
	{

		public abstract AbstractRepositoryDatabase<T> GetDatabase();

		protected abstract R Resolve(T obj);

		protected abstract AbstractStorage<T, R, D, V> CreateInstance();


		public async Task<List<R>> Repositories()
		{
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

