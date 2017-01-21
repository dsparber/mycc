using System;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;

namespace MyCC.Core.Currency.Repositories
{
    public class LocalCurrencyRepository : CurrencyRepository
    {
        public LocalCurrencyRepository(int id) : base(id) { }
        public override int RepositoryTypeId => CurrencyRepositoryDbm.DB_TYPE_LOCAL_REPOSITORY;

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

        protected override Func<Model.Currency, bool> DatabaseFilter
        {
            get { return v => true; }
        }
    }
}

