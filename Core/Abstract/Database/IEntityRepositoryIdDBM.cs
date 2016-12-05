
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Database.Interfaces
{
	public interface IEntityRepositoryIdDBM<T, IDType> : IEntityDBM<T, IDType> where T : PersistableRepositoryElement<IDType>
	{
		int RepositoryId { get; set; }
	}
}

