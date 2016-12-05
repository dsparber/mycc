
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Database.Interfaces
{
	public interface IEntityRepositoryIdDBM<T, IDType> : IEntityDBM<T, IDType> where T : IPersistableWithParent<IDType>
	{
		int ParentId { get; set; }
	}
}

