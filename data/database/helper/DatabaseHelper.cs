using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using data.database.interfaces;

namespace data.database.helper
{
	public static class DatabaseHelper
	{
		public static async Task InsertOrUpdate<T, V>(AbstractEntityDatabase<T, V> database, IEnumerable<T> insertOrUpdateObjects) where T : IEntityDBM<V>
		{
			var existingDbObjects = await database.GetAllDbObjects();

			var existingObjectsIdMap = (await Task.WhenAll(existingDbObjects.Select(async o => new Tuple<T, V>(o, await o.Resolve())))).ToList();
			var comparisonObjectsIdMap = (await Task.WhenAll(insertOrUpdateObjects.Select(async o => new Tuple<T, V>(o, await o.Resolve())))).ToList();

			var existingObjects = existingObjectsIdMap.Select(e => e.Item2);
			var comparisonObjects = comparisonObjectsIdMap.Select(c => c.Item2);

			var objectsFound = comparisonObjects.ToList().FindAll(existingObjects.Contains);
			var objectsNotFound = comparisonObjects.ToList().FindAll(c => !existingObjects.Contains(c));

			var dbObjectsFound = objectsFound.Select(o =>
			{
				var dbObj = comparisonObjectsIdMap.Find(e => e.Item2.Equals(o)).Item1;
				dbObj.Id = existingObjectsIdMap.Find(e => e.Item2.Equals(o)).Item1.Id;
				return dbObj;
			});
			var dbObjectsNotFound = objectsNotFound.Select(o => comparisonObjectsIdMap.Find(e => e.Item2.Equals(o)).Item1);

			var connection = await database.Connection();
			await connection.InsertAllAsync(dbObjectsNotFound);
			await connection.UpdateAllAsync(dbObjectsFound);
		}

		public static async Task Delete<T, V>(AbstractEntityDatabase<T, V> database, T objectToDelete) where T : IEntityDBM<V>
		{
			var connection = await database.Connection();
			await connection.DeleteAsync(objectToDelete);
		}
	}
}

