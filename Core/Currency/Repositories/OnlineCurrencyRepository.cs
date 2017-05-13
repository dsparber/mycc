using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Storage;

namespace MyCC.Core.Currency.Repositories
{
    public abstract class OnlineCurrencyRepository : CurrencyRepository
    {
        protected OnlineCurrencyRepository(int id) : base(id) { }

        public override async Task<bool> LoadFromDatabase()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        public abstract string Description { get; }

        protected abstract Task<IEnumerable<Model.Currency>> GetCurrencies();

        public override async Task<bool> FetchOnline()
        {
            var currentElements = (await GetCurrencies()).ToList();
            var mapElements = new List<CurrencyMapDbm>();
            var existingElements = new List<Model.Currency>();

            if (currentElements == null) return false;

            var existingCurrencies = CurrencyStorage.Instance.LocalRepository.Elements.ToList();

            foreach (var c in currentElements)
            {
                var existing = existingCurrencies.Find(c.Equals);

                if (existing != null)
                {
                    var newName = c.Name.ToLower().Equals(c.Code.ToLower()) ? existing.Name : c.Name;
                    if (!string.Equals(newName, existing.Name))
                    {
                        existing.Name = newName;
                        existingElements.Add(existing);
                    }
                }

                var mapEntry = new CurrencyMapDbm { Code = c.Code, ParentId = Id };
                mapElements.Add(mapEntry);
            }
            try
            {
                await CurrencyStorage.Instance.LocalRepository.Add(currentElements.Except(existingCurrencies));
                await CurrencyStorage.Instance.LocalRepository.Update(existingElements);
                await CurrencyRepositoryMapStorage.Instance.LocalRepository.Add(mapElements);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }


            // var toDelete = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == Id).Where(e => !mapElements.Contains(e)).ToList();
            // await Task.WhenAll(toDelete.Select(e => CurrencyRepositoryMapStorage.Instance.LocalRepository.Remove(e)));
            // await Task.WhenAll(Elements.Where(e => toDelete.Contains(new CurrencyMapDbm { Code = e.Code, ParentId = Id })).Select(Remove));

            LastFetch = DateTime.Now;
            return true;
        }
    }
}

