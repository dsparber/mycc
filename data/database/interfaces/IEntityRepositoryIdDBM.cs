
using MyCryptos.models;

namespace data.database.interfaces
{
	public interface IEntityRepositoryIdDBM<T, IDType> : IEntityDBM<T, IDType> where T : PersistableRepositoryElement<IDType>
	{
		int RepositoryId { get; set; }
	}
}

