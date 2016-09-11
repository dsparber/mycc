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
	}


}