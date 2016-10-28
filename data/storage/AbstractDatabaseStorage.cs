using data.repositories.general;
using data.database.interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using MyCryptos.models;
using MyCryptos.data.database.helper;

namespace data.storage
{
	public abstract class AbstractDatabaseStorage<Ta, Va, T, V> : AbstractStorage<Ta, Va> where Va : AbstractDatabaseRepository<T,V> where Ta : IEntityDBM<Va> where T : IEntityRepositoryIdDBM<V> where V : PersistableRepositoryElement
	{
		protected AbstractDatabaseStorage(AbstractDatabase<Ta, Va> database) : base(database)
		{
			CachedElements = new List<V>();
			CachedElementsWithRepository = new List<Tuple<V, Va>>();
		}

		public List<V> CachedElements { get; private set; }
		public List<Tuple<V, Va>> CachedElementsWithRepository { get; private set; }

		public async Task<List<V>> AllElements()
		{
			return (await Repositories()).SelectMany(r => r.Elements).ToList();
		}

		public async Task<List<Tuple<V, Va>>> AllElementsWithRepositories()
		{
			return (await Repositories()).SelectMany(r => r.Elements.Select(e => Tuple.Create(e, r))).ToList();
		}

		public async Task<List<V>> AllOfType<A>()
		{
			return (await AllElements()).FindAll(e => e is A);
		}

		public async override Task Fetch()
		{
			await base.Fetch();
			await updateCache();
		}

		public async Task updateCache()
		{
			CachedElements = await AllElements();
			CachedElementsWithRepository = await AllElementsWithRepositories();
		}

		public async override Task FetchFast()
		{
			await base.FetchFast();
			await updateCache();
		}

		public override async Task Remove(Va repository)
		{
			await base.Remove(repository);

			// Delete all elements
			var repo = (await Repositories()).Find(r => r.Id == repository.Id);
			await repo.RemoveAll();

			await updateCache();
		}

		public async override Task Update(Va repository)
		{
			await base.Update(repository);
			await updateCache();
		}

		public async override Task Add(Va repository)
		{
			await base.Add(repository);
			await updateCache();
		}

		public abstract Task<Va> GetLocalRepository();

		public async Task AddToLocalRepository(V element)
		{
			var local = await GetLocalRepository();
			if (local != null)
			{
				await local.Add(element);
				await updateCache();
			}
		}

		public async Task RemoveFromLocalRepository(V element)
		{
			var local = await GetLocalRepository();
			if (local != null)
			{
				await local.Remove(element);
				await updateCache();
			}
		}
	}
}