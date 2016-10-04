using System.Threading.Tasks;

namespace data.repositories.general
{
	public abstract class AbstractRepository<V>
	{
		public string Name;
		public int RepositoryTypeId;

		public abstract Task<bool> Fetch();

		public abstract Task<bool> FetchFast();

		protected AbstractRepository(int repositoryTypeId, string name)
		{
			RepositoryTypeId = repositoryTypeId;
			Name = name;
		}
	}
}