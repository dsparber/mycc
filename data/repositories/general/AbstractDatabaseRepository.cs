﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using data.database.helper;
using data.database.interfaces;

namespace data.repositories.general
{
	public abstract class AbstractDatabaseRepository<T, V> : AbstractRepository<V> where T : IEntityRepositoryIdDBM<V>
	{
		public List<V> Elements;

		public DateTime LastFastFetch { get; protected set; }
		public DateTime LastFetch { get; protected set; }

		public int DatabaseId { get; set; }

		protected abstract AbstractEntityRepositoryIdDatabase<T, V> GetDatabase();

		protected AbstractDatabaseRepository(int repositoryId, string name) : base(repositoryId, name)
		{
			Elements = new List<V>();
		}

		protected async Task<bool> FetchFromDatabase()
		{
			try
			{
				var db = GetDatabase();
				Elements = new List<V>(await db.GetAll(Id));
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}

		protected async Task WriteToDatabase()
		{
			var db = GetDatabase();
			Elements = Elements.Distinct().ToList();
			await db.Write(Elements, Id);
			Elements = new List<V>(await db.GetAll(Id));
		}

		protected async Task DeleteFromDatabase(V element)
		{
			var db = GetDatabase();
			await db.Delete(element, Id);
			Elements = new List<V>(await db.GetAll(Id));
		}

		public async Task Add(V element)
		{
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