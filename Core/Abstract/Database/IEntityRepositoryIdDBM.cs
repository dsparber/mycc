
using MyCryptos.Core.Abstract.Models;

namespace MyCryptos.Core.Abstract.Database
{
    public interface IEntityRepositoryIdDBM<T, IDType> : IEntityDBM<T, IDType> where T : IPersistableWithParent<IDType>
    {
        int ParentId { get; set; }
    }
}

