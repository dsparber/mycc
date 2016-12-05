using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Models;

namespace MyCryptos.Core.Abstract.Repositories
{
	public abstract class AbstractRepository : Persistable<int>
	{
		public abstract int RepositoryTypeId { get; }

		public int Id { get; set; }

		public abstract Task<bool> FetchOnline();

		public abstract Task<bool> LoadFromDatabase();

		protected AbstractRepository(int id)
		{
			Id = id;
		}
	}
}