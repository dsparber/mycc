using System;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
    public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
    {
        protected OnlineExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name) { }

        public override async Task<bool> FetchFast()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        protected abstract Task GetFetchTask(ExchangeRate exchangeRate);

        private async Task FetchWithFiler(Func<ExchangeRate, bool> filter)
        {
            var newElements = Elements.Where(e => e.ReferenceCurrency != null && e.SecondaryCurrency != null).Where(filter).ToList();

            await Task.WhenAll(newElements.Select(GetFetchTask));
            await Task.WhenAll(newElements.Select(AddOrUpdate));

            LastFetch = DateTime.Now;
        }

        public override async Task<bool> Fetch()
        {
            await FetchWithFiler(e => true);
            return true;
        }

        public override async Task<bool> FetchNew()
        {
            await FetchWithFiler(e => !e.Rate.HasValue);
            return true;
        }
    }
}

