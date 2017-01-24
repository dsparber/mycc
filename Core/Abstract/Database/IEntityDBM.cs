using System.Threading.Tasks;
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Abstract.Database
{
    public interface IEntityDbm<T, TIdType> where T : IPersistable<TIdType>
    {
        TIdType Id { get; set; }

        Task<T> Resolve();
    }
}

