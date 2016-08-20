using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.interfaces;

namespace data.repositories.general
{
	public abstract class AbstractRepository<T, V> where T : IEntityRepositoryIdDBM<V>
	{
		public List<V> Elements;
		public string Name;
		public int Id; 

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected abstract AbstractEntityRepositoryIdDatabase<T, V> GetDatabase();

		protected AbstractRepository(int repositoryId, string name)
		{
			Id = repositoryId;
			Name = name;
			Elements = new List<V>();
		}

		protected async Task FetchFromDatabase()
		{
			var db = GetDatabase();
			Elements = new List<V>(await db.GetAll(Id));
		}

		protected async Task WriteToDatabase()
		{
			var db = GetDatabase();
			await db.Write(Elements, Id);
		}

		public async Task Add(V element) {
			Elements.Add(element);
			await WriteToDatabase();
		}
	}
}

