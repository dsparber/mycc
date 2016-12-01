using System.Threading.Tasks;
using MyCryptos.models;

namespace data.database.interfaces
{
	public interface IEntityDBM<T, IDType> where T : Persistable<IDType>
	{
		IDType Id { get; set; }

		Task<T> Resolve();
	}
}

