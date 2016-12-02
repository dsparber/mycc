using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Storage;
using Xamarin.Forms;

namespace MyCryptos.Core.Repositories.Currency
{
    public abstract class OnlineCurrencyRepository : CurrencyRepository
    {
        protected OnlineCurrencyRepository(int repositoryId, string name) : base(repositoryId, name) { }

        public override async Task<bool> FetchFast()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        protected abstract Task<IEnumerable<Models.Currency>> GetCurrencies();

        public override async Task<bool> Fetch()
        {
            var currentElements = await GetCurrencies();
            var mapElements = new List<CurrencyRepositoryElementDBM>();

            if (currentElements == null) return false;
            foreach (var c in currentElements)
            {
                var existing = CurrencyStorage.Instance.LocalRepository.Elements.ToList().Find(e => c.Equals(e));

                if (existing != null)
                {
                    c.Name = (c.Name.Equals(c.Code)) ? existing.Name : c.Name;
                }
                await CurrencyStorage.Instance.LocalRepository.AddOrUpdate(c);

                var mapEntry = new CurrencyRepositoryElementDBM { Code = c.Code, RepositoryId = Id };
                var all = CurrencyRepositoryMapStorage.Instance.AllElements;
                var existingMapEntry = all.Find(e => e.Code.Equals(c.Code) && e.RepositoryId == Id);

                mapElements.Add(existingMapEntry ?? mapEntry);

                if (existingMapEntry == null)
                {
                    await CurrencyRepositoryMapStorage.Instance.LocalRepository.Add(mapEntry);
                }
            }

            var toDelete = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.RepositoryId == Id).Where(e => !mapElements.Contains(e));
            await Task.WhenAll(toDelete.Select(e => CurrencyRepositoryMapStorage.Instance.LocalRepository.Remove(e)));
            await Task.WhenAll(Elements.Where(e => toDelete.Contains(new CurrencyRepositoryElementDBM { Code = e.Code, RepositoryId = Id })).Select(Remove));

            LastFetch = DateTime.Now;
            return true;
        }
    }
}

