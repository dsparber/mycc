using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Abstract.Models;
using MyCC.Core.Abstract.Repositories;

namespace MyCC.Core.Abstract.Storage
{
    public abstract class AbstractDatabaseStorage<TA, TVa, T, TV, TIdType> : AbstractStorage<TA, TVa> where TVa : AbstractDatabaseRepository<T, TV, TIdType> where TA : IEntityDbm<TVa, int> where T : IEntityRepositoryIdDbm<TV, TIdType> where TV : IPersistableWithParent<TIdType>
    {
        protected AbstractDatabaseStorage(AbstractDatabase<TA, TVa, int> database) : base(database) { }

        public List<TV> AllElements => Repositories.ToList().SelectMany(r => r.Elements).ToList();
    }
}