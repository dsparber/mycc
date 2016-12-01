using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Database.Interfaces
{
    public interface IEntityDBM<T, IDType> where T : Persistable<IDType>
    {
        IDType Id { get; set; }

        Task<T> Resolve();
    }
}

