using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;

namespace MyCryptos.Core.Repositories.Currency
{
    public class LocalCurrencyRepository : CurrencyRepository
    {
        public LocalCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

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

        protected override Func<Models.Currency, bool> DatabaseFilter
        {
            get { return v => true; }
        }
    }
}

