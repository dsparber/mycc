﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Core;

namespace MyCryptos.Core.Storage
{
    public abstract class AbstractDatabaseStorage<Ta, Va, T, V, IdType> : AbstractStorage<Ta, Va> where Va : AbstractDatabaseRepository<T, V, IdType> where Ta : IEntityDBM<Va, int> where T : IEntityRepositoryIdDBM<V, IdType> where V : PersistableRepositoryElement<IdType>
    {
        protected AbstractDatabaseStorage(AbstractDatabase<Ta, Va, int> database) : base(database) { }

        public List<V> AllElements
        {
            get
            {
                return Repositories.SelectMany(r => r.Elements).ToList();
            }
        }

        public List<Tuple<V, Va>> AllElementsWithRepositories
        {
            get
            {
                return Repositories.SelectMany(r => r.Elements.Select(e => Tuple.Create(e, r))).ToList();
            }
        }

        public List<V> AllOfType<A>()
        {
            return AllElements.FindAll(e => e is A);
        }

        public V Find(V element)
        {
            return AllElements.Find(e => e.Equals(element));
        }

        public override async Task Remove(Va repository)
        {
            // Delete all elements
            var repo = Repositories.Find(r => r.Id == repository.Id);
            if (repo != null) await repo.RemoveAll();

            await base.Remove(repository);
        }

        public abstract Va LocalRepository { get; }
    }
}