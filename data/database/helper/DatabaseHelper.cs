using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using data.database.interfaces;

namespace data.database.helper
{
	public class DatabaseHelper
	{
		DatabaseHelper(){}

		public static async Task InsertOrUpdate<T, V>(AbstractEntityDatabase<T, V> database, T obj) where T : IEntityDBM<V>
		{
			var existingObjects = await database.GetAllDbObjects();

			var comparisonObj = await obj.Resolve();
			var resolved = await Task.WhenAll(existingObjects.Select(async o => new Tuple<int, V>(o.Id, await o.Resolve())));
			var id = resolved.ToList().Find(r => r.Item2.Equals(comparisonObj)).Item1;

			var existingObject = existingObjects.ToList().Find(o => o.Id == id);

			if (!(EqualityComparer<T>.Default.Equals(existingObject, default(T))))
			{
				obj.Id = existingObject.Id;
				await (await database.Connection()).UpdateAsync(obj);
			}
			else {
				await (await database.Connection()).InsertAsync(obj);
			}
		}
	}
}

