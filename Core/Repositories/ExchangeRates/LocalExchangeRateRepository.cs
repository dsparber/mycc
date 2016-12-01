using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
    public class LocalExchangeRateRepository : ExchangeRateRepository
    {
        public LocalExchangeRateRepository(string name) : base(ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

        public override async Task<bool> Fetch()
        {
            LastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        public override async Task<bool> FetchFast()
        {
            LastFastFetch = DateTime.Now;
            return await Fetch();
        }

        public override async Task<bool> FetchNew()
        {
            LastFastFetch = DateTime.Now;
            return await Fetch();
        }
    }
}