using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Currencies.Sources;
using MyCC.Core.Database;
using MyCC.Core.Helpers;
using SQLite;

namespace MyCC.Core.Currencies
{
    public class CurrencyStorage
    {
        private readonly IEnumerable<ICurrencySource> _sources;
        internal Dictionary<string, Currency> CurrencyDictionary;

        public IEnumerable<Currency> Currencies { get; private set; }

        private readonly SQLiteAsyncConnection _connection;

        private bool _loadedFromDatabase;

        private CurrencyStorage()
        {
            _sources = new List<ICurrencySource>
            {
                new BittrexCurrencySource(),
                new BlockExpertsCurrencySource(),
                new CryptonatorCurrencySource(),
                new CryptoIdCurrencySource(),
                new OpenexchangeCurrencySource()
            };
            _connection = DatabaseUtil.Connection;

            CurrencyDictionary = new Dictionary<string, Currency>();
            Currencies = new Currency[] { };
        }

        private static CurrencyStorage _instance;
        public static CurrencyStorage Instance => _instance ?? (_instance = new CurrencyStorage());


        public async Task LoadOnline(Action<double, string> onProgress = null, Action onDataOperationsFinished = null)
        {
            if (!_loadedFromDatabase) await LoadFromDatabase();

            var newCurrencies = new List<Currency>();
            var fetchedCurrencies = new List<Currency>();
            var updateCurrencies = new List<Currency>();

            var progress = .0;
            foreach (var source in _sources)
            {
                var result = (await source.GetCurrencies()).ToList();

                var allCurrencies = fetchedCurrencies.Concat(Currencies).Distinct().ToList();

                var overlap = allCurrencies.Intersect(result).Select(c => c.Merge(result.Find(x => x.Id == c.Id)));
                var update = overlap.Where(t => t.Item1).Select(t => t.Item2).ToList();

                var duplicates = fetchedCurrencies.Intersect(result).Select(c => c.Merge(result.Find(x => x.Id == c.Id))).Select(c => c.Item2).ToList();
                fetchedCurrencies = fetchedCurrencies.Except(duplicates).Concat(result.Except(duplicates)).Concat(duplicates).ToList();
                newCurrencies.AddRange(result.Except(allCurrencies));
                updateCurrencies = updateCurrencies.Except(update).Concat(update).ToList();

                progress += 1;
                onProgress?.Invoke(progress / _sources.Count(), source.Name);
            }

            var oldElemets = Currencies.Except(fetchedCurrencies);

            Currencies = fetchedCurrencies;
            CurrencyDictionary = fetchedCurrencies.ToDictionary(c => c.Id, c => c);

            await _connection.InsertAllAsync(newCurrencies.Select(c => c.DbObject));
            await _connection.UpdateAllAsync(updateCurrencies.Select(c => c.DbObject));
            await Task.WhenAll(oldElemets.Select(c => _connection.DeleteAsync(c.DbObject)));
            onDataOperationsFinished?.Invoke();
        }

        public async Task LoadFromDatabase()
        {
            await _connection.CreateTableAsync<CurrencyDbm>();

            var currencies = (await _connection.Table<CurrencyDbm>().ToListAsync()).Select(c => c.Currency).ToList();
            Currencies = currencies;
            CurrencyDictionary = currencies.ToDictionary(c => c.Id, c => c);
            _loadedFromDatabase = true;
        }
    }
}