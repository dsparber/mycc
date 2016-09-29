﻿using data.repositories.general;
using data.database.interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace data.storage
{
	public abstract class AbstractDatabaseStorage<T, R, D, V> : AbstractStorage<T, R, V> where R : AbstractDatabaseRepository<D, V> where D : IEntityRepositoryIdDBM<V>
	{
		protected AbstractDatabaseStorage()
		{
			CachedElements = new List<V>();
			CachedElementsWithRepository = new List<Tuple<V, R>>();
		}

		public List<V> CachedElements { get; private set; }
		public List<Tuple<V, R>> CachedElementsWithRepository { get; private set; }

		public async Task<List<V>> AllElements()
		{
			return (await Repositories()).SelectMany(r => r.Elements).ToList();
		}

		public async Task<List<Tuple<V, R>>> AllElementsWithRepositories()
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
			CachedElements = await AllElements();
			CachedElementsWithRepository = await AllElementsWithRepositories();
		}

		public async override Task FetchFast()
		{
			await base.FetchFast();
			CachedElements = await AllElements();
			CachedElementsWithRepository = await AllElementsWithRepositories();
		}
	}
}