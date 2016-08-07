using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.interfaces;

namespace data.repositories.general
{
	public abstract class AbstractRepository<T, V> where T : IEntityRepositoryIdDBM<V>
	{
		public List<V> Elements;
		public string RepositoryName;
		public int RepositoryId; 

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected abstract AbstractEntityRepositoryIdDatabase<T, V> GetDatabase();

		protected AbstractRepository(int repositoryId)
		{
			RepositoryId = repositoryId;
			Elements = new List<V>();
		}

		protected async Task FetchFromDatabase()
		{
			var db = GetDatabase();
			Elements = new List<V>(await db.GetAll(RepositoryId));
		}

		protected async Task WriteToDatabase()
		{
			var db = GetDatabase();
			await db.Write(Elements, RepositoryId);
		}
	}
}

