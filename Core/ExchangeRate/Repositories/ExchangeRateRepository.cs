using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.ExchangeRate.Database;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
	public abstract class ExchangeRateRepository : AbstractDatabaseRepository<ExchangeRateDbm, Model.ExchangeRate, string>
	{
		protected ExchangeRateRepository(int id) : base(id, new ExchangeRateDatabase()) { }

		public abstract Task<bool> FetchNew();
	}
}