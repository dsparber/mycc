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
	public abstract class AbstractStorage<T, V> where V : AbstractRepository where T : IEntityDBM<V, int>
	{
		public List<V> Repositories { get; private set; }

		readonly Task onCreationTask;

		protected AbstractStorage(AbstractDatabase<T, V, int> database)
		{
			Database = database;
			Repositories = new List<V>();
			onCreationTask = OnCreation();
		}

		AbstractDatabase<T, V, int> Database { get; set; }

		async Task OnCreation()
		{
			Repositories.AddRange(await Database.GetAll());
			if (ApplicationSettings.FirstLaunch)
			{
				await OnFirstLaunch();
			}
		}
		protected virtual Task OnFirstLaunch() { return Task.Factory.StartNew(() => { }); }


		public virtual async Task Add(V repository)
		{
			repository = await Database.Insert(repository);
			Repositories.Add(repository);
		}

		public virtual async Task Remove(V repository)
		{
			await Database.Delete(repository);
			Repositories.Remove(repository);
		}

		public virtual async Task Update(V repository)
		{
			Repositories.Remove(repository);
			repository = await Database.Update(repository);
			Repositories.Add(repository);
		}

		public List<A> RepositoriesOfType<A>() where A : V
		{
			return Repositories.OfType<A>().ToList();
		}

		public List<V> RepositoriesOfType(Type type)
		{
			return Repositories.FindAll(r => r.GetType() == type);
		}

		public A RepositoryOfType<A>() where A : V
		{
			return RepositoriesOfType<A>().FirstOrDefault();
		}

		public V RepositoryOfType(Type type)
		{
			return RepositoriesOfType(type).FirstOrDefault();
		}

		protected virtual Task beforeFetching() {
			return Task.Factory.StartNew(() => { });
		}

		protected virtual Task beforeFastFetching()
		{
			return Task.Factory.StartNew(() => { });
		}

		public async Task Fetch()
		{
			await onCreationTask;
			await beforeFetching();
			await Task.WhenAll(Repositories.Select(x => x.Fetch()));
		}

		public async Task FetchFast()
		{
			await onCreationTask;
			await beforeFastFetching();
			await Task.WhenAll(Repositories.Select(x => x.FetchFast()));
		}
	}
}