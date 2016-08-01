using System.Threading.Tasks;

namespace data.database.interfaces
{
	public interface IDBM<T>
	{
		int Id { get; set; }

		Task<T> Resolve();
	}
}

