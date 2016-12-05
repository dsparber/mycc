using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Models;

namespace MyCryptos.Core.Abstract.Database
{
    public interface IEntityDBM<T, IDType> where T : Persistable<IDType>
    {
        IDType Id { get; set; }

        Task<T> Resolve();
    }
}

