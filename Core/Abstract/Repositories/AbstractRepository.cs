using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Repositories.Core
{
	public abstract class AbstractRepository : Persistable<int>
	{
		public string Name;
		public int RepositoryTypeId;

		public int Id { get; set; }

		public abstract Task<bool> FetchOnline();

		public abstract Task<bool> LoadFromDatabase();

		protected AbstractRepository(int repositoryTypeId, string name)
		{
			RepositoryTypeId = repositoryTypeId;
			Name = name;
		}
	}
}