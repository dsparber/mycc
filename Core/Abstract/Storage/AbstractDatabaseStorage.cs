using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using MyCC.Core.Abstract.Repositories;

namespace MyCC.Core.Abstract.Storage
{
    public abstract class AbstractDatabaseStorage<TA, TVa, T, TV, TIdType> : AbstractStorage<TA, TVa> where TVa : AbstractDatabaseRepository<T, TV, TIdType> where TA : IEntityDbm<TVa, int> where T : IEntityRepositoryIdDbm<TV, TIdType> where TV : IPersistableWithParent<TIdType>
    {
        protected AbstractDatabaseStorage(AbstractDatabase<TA, TVa, int> database) : base(database) { }

        public List<TV> AllElements
        {
            get
            {
                return Repositories.SelectMany(r => r.Elements).ToList();
            }
        }

        public List<Tuple<TV, TVa>> AllElementsWithRepositories
        {
            get
            {
                return Repositories.SelectMany(r => r.Elements.Select(e => Tuple.Create(e, r))).ToList();
            }
        }

        public TV Find(TV element)
        {
            return AllElements.Find(e => Equals(e, element));
        }

        public override async Task Remove(TVa repository)
        {
            // Delete all elements
            var repo = Repositories.Find(r => r.Id == repository.Id);
            if (repo != null) await repo.RemoveAll();

            await base.Remove(repository);
        }

        public abstract TVa LocalRepository { get; }
    }
}