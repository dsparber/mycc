using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using MyCC.Core.Abstract.Repositories;

namespace MyCC.Core.Abstract.Storage
{
    public abstract class AbstractDatabaseStorage<Ta, Va, T, V, IdType> : AbstractStorage<Ta, Va> where Va : AbstractDatabaseRepository<T, V, IdType> where Ta : IEntityDBM<Va, int> where T : IEntityRepositoryIdDBM<V, IdType> where V : IPersistableWithParent<IdType>
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
            return AllElements.Find(e => Equals(e, element));
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