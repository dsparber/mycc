using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Currency.Database;

namespace MyCryptos.Core.Currency.Repositories
{
    public class CurrencyRepositoryMap : AbstractDatabaseRepository<CurrencyMapDBM, CurrencyMapDBM, string>
    {
        public CurrencyRepositoryMap() : base(0, null, new CurrencyMapDatabase()) { }

        public async override Task<bool> FetchOnline()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        public async override Task<bool> LoadFromDatabase()
        {
            return await FetchOnline();
        }

        protected override Func<CurrencyMapDBM, bool> DatabaseFilter => v => true;
    }
}