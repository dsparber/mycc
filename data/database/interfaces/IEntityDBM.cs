using System.Threading.Tasks;

namespace data.database.interfaces
{
	public interface IEntityDBM<T>
	{
		int Id { get; set; }

		Task<T> Resolve();
	}
}

