using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Currency.Database;
using MyCryptos.Core.Currency.Storage;

namespace MyCryptos.Core.Currency.Repositories
{
	public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDbm, Model.Currency, string>
	{
		protected CurrencyRepository(int id) : base(id, new CurrencyDatabase()) { }

		public List<Model.Currency> Currencies
		{
			get
			{
				var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == Id).Select(e => e.Code);
				return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code))).ToList();
			}
		}
	}
}

