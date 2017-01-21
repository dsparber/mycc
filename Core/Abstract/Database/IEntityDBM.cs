using System.Threading.Tasks;
using MyCC.Core.Abstract.Models;

namespace MyCC.Core.Abstract.Database
{
    public interface IEntityDBM<T, IDType> where T : Persistable<IDType>
    {
        IDType Id { get; set; }

        Task<T> Resolve();
    }
}

