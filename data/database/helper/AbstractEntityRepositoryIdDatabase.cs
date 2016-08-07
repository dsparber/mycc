using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.interfaces;

namespace data.database.helper
{
	public abstract class AbstractEntityRepositoryIdDatabase<T, V> : AbstractEntityDatabase<T, V> where T : IEntityRepositoryIdDBM<V>
	{
		public abstract Task Write(IEnumerable<V> data, int repositoryId);

		public async Task<IEnumerable<T>> GetAllDbObjects(int repositoryId)
		{
			return (await GetAllDbObjects()).Where(o => o.RepositoryId == repositoryId);
		}

		public virtual async Task<IEnumerable<V>> GetAll(int repositoryId)
		{
			return await Task.WhenAll((await GetAllDbObjects(repositoryId)).Select(o => o.Resolve()));
		}

		public sealed override async Task Write(IEnumerable<V> data)
		{
			await Write(data, 0);
		}
	}
}

