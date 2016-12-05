using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Currency.Database;

namespace MyCryptos.Core.Currency.Repositories
{
	public class CurrencyRepositoryMap : AbstractDatabaseRepository<CurrencyMapDbm, CurrencyMapDbm, string>
	{
		public CurrencyRepositoryMap() : base(default(int), new CurrencyMapDatabase()) { }
		public override int RepositoryTypeId => 1;

		public override async Task<bool> FetchOnline()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public override async Task<bool> LoadFromDatabase()
		{
			return await FetchOnline();
		}

		protected override Func<CurrencyMapDbm, bool> DatabaseFilter => v => true;
	}
}