using System.Threading.Tasks;

namespace data.repositories.general
{
	public abstract class AbstractRepository<V>
	{
		public string Name;
		public int Id;

		public abstract Task<bool> Fetch();

		public abstract Task<bool> FetchFast();

		protected AbstractRepository(int repositoryId, string name)
		{
			Id = repositoryId;
			Name = name;
		}
	}
}