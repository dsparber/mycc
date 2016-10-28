using System.Threading.Tasks;
using data.database;
using data.database.models;
using data.repositories.general;
using MyCryptos.models;

namespace data.repositories.exchangerate
{
	public abstract class ExchangeRateRepository : AbstractDatabaseRepository<ExchangeRateDBM, ExchangeRate>
	{
		protected ExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name, new ExchangeRateDatabase()) { }

		public abstract Task<bool> FetchNew();
	}
}