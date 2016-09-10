using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.helper;
using data.database.interfaces;

namespace data.repositories.general
{
	public abstract class AbstractRepository<T, V> where T : IEntityRepositoryIdDBM<V>
	{
		public DateTime LastFastFetch{ get; protected set; }
		public DateTime LastFetch { get; protected set; }

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
			Elements = Elements.Distinct().ToList();
			await db.Write(Elements, Id);
			Elements = new List<V>(await db.GetAll(Id));
		}

		protected async Task DeleteFromDatabase(V element) {
			var db = GetDatabase();
			await db.Delete(element, Id);
			Elements = new List<V>(await db.GetAll(Id));
		}

		public async Task Add(V element) {
			Elements.Add(element);
			await WriteToDatabase();
		}

		public async Task Update(V element)
		{
			Elements.Remove(element);
			Elements.Add(element);
			await WriteToDatabase();
		}

		public async Task Delete(V element)
		{
			await DeleteFromDatabase(element);
		}
	}
}

