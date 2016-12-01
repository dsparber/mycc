using System.Threading.Tasks;
using MyCryptos.models;

namespace data.repositories.general
{
	public abstract class AbstractRepository : Persistable<int>
	{
		public string Name;
		public int RepositoryTypeId;

		public int Id { get; set; }

		public abstract Task<bool> Fetch();

		public abstract Task<bool> FetchFast();

		protected AbstractRepository(int repositoryTypeId, string name)
		{
			RepositoryTypeId = repositoryTypeId;
			Name = name;
		}
	}
}