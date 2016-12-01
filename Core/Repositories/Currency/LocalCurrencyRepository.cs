using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;

namespace MyCryptos.Core.Repositories.Currency
{
    public class LocalCurrencyRepository : CurrencyRepository
    {
        public LocalCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

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

        protected override Func<Models.Currency, bool> DatabaseFilter
        {
            get { return v => true; }
        }
    }
}

