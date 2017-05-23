using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Currencies.Sources;
using MyCC.Core.Helpers;
using SQLite;
using Xamarin.Forms;

namespace MyCC.Core.Currencies
{
    public class CurrencyStorage
    {
        public readonly IEnumerable<ICurrencySource> CurrencySources;

        public Dictionary<string, Currency> CurrencyDictionary { get; private set; }
        public IEnumerable<Currency> Currencies { get; private set; }

        private readonly SQLiteAsyncConnection _connection;

        private bool _loadedFromDatabase;

        private CurrencyStorage()
        {
            CurrencySources = new List<ICurrencySource>
            {
                new BittrexCurrencySource(),
                new BlockExpertsCurrencySource(),
                new CryptonatorCurrencySource(),
                new CryptoIdCurrencySource(),
                new OpenexchangeCurrencySource()
            };
            _connection = DependencyService.Get<ISqLiteConnection>().Connection;

            CurrencyDictionary = new Dictionary<string, Currency>();
            Currencies = new Currency[] { };
        }

        private static CurrencyStorage _instance;
        public static CurrencyStorage Instance => _instance ?? (_instance = new CurrencyStorage());


        public async Task LoadOnline(Action<ICurrencySource> onStartedFetching = null, Action<ICurrencySource> onFinishedFetching = null, Action onDataOperationsFinished = null)
        {
            if (!_loadedFromDatabase) await LoadFromDatabase();

            var newCurrencies = new List<Currency>();
            var fetchedCurrencies = new List<Currency>();
            var updateCurrencies = new List<Currency>();

            foreach (var source in CurrencySources)
            {
                onStartedFetching?.Invoke(source);
                var result = (await source.GetCurrencies()).ToList();

                var allCurrencies = fetchedCurrencies.Concat(Currencies).Distinct().ToList();

                var overlap = allCurrencies.Intersect(result).Select(c => c.Merge(result.Find(x => x.Id == c.Id)));
                var update = overlap.Where(t => t.Item1).Select(t => t.Item2).ToList();

                fetchedCurrencies = fetchedCurrencies.Except(result).Concat(result).ToList();
                newCurrencies.AddRange(result.Except(allCurrencies));
                updateCurrencies = updateCurrencies.Except(update).Concat(update).ToList();

                onFinishedFetching?.Invoke(source);
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


        public static Currency Find(string id)
        {
            Instance.CurrencyDictionary.TryGetValue(id, out var currency);
            return currency ?? Instance.Currencies.FirstOrDefault(c => c.Id.Equals(id)) ?? new Currency(id);
        }

        public static Currency Find(string code, bool isCrypto) => Find($"{code}{(isCrypto ? 1 : 0)}");

        public static IEnumerable<Currency> CurrenciesOf(int flags) => Instance.Currencies.Where(c => c.BalanceSourceFlags.IsSet(flags));

    }
}