
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Abstract.Database
{
    public interface IEntityRepositoryIdDBM<T, IDType> : IEntityDBM<T, IDType> where T : IPersistableWithParent<IDType>
    {
        int ParentId { get; set; }
    }
}

