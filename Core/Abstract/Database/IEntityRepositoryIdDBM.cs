
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Abstract.Database
{
    public interface IEntityRepositoryIdDbm<T, TIdType> : IEntityDbm<T, TIdType> where T : IPersistableWithParent<TIdType>
    {
        int ParentId { get; set; }
    }
}

