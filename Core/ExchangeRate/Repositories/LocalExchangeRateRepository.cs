using System;
using System.Threading.Tasks;
using MyCryptos.Core.ExchangeRate.Database;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
	public class LocalExchangeRateRepository : ExchangeRateRepository
	{
		public LocalExchangeRateRepository(int id) : base(id) { }
		public override int RepositoryTypeId => ExchangeRateRepositoryDbm.DB_TYPE_LOCAL_REPOSITORY;

		public override async Task<bool> FetchOnline()
		{
			LastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public override async Task<bool> LoadFromDatabase()
		{
			LastFastFetch = DateTime.Now;
			return await FetchOnline();
		}

		public override async Task<bool> FetchNew()
		{
			LastFastFetch = DateTime.Now;
			return await FetchOnline();
		}
	}
}