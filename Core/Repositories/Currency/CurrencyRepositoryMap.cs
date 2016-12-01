using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Core;

namespace MyCryptos.Core.Repositories.Currency
{
    public class CurrencyRepositoryMap : AbstractDatabaseRepository<CurrencyRepositoryElementDBM, CurrencyRepositoryElementDBM, string>
    {
        public CurrencyRepositoryMap() : base(0, null, new CurrencyMapDatabase()) { }

        public async override Task<bool> Fetch()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        public async override Task<bool> FetchFast()
        {
            return await Fetch();
        }

        protected override Func<CurrencyRepositoryElementDBM, bool> DatabaseFilter
        {
            get { return v => true; }
        }
    }
}

