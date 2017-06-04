using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Database;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Implementations;
using MyCC.Core.Settings;

namespace MyCC.Core.Rates
{
    public static class RateStorage
    {
        private static readonly object SaveToDatabaseLock = new object();
        private static bool LoadedFromDatabase;

        private static readonly IEnumerable<IRateSource> Sources = new List<IRateSource> {
            new BittrexExchangeRateSource(),
            new BtceExchangeRateSource(),
            new CryptonatorExchangeRateSource(),
            new FixerIoExchangeRateSource(),
            new BitstampExchangeRateSource(),
            new KrakenExchangeRateSource(),
            new QuadrigaCxExchangeRateSource(),
            new CoinbaseExchangeRateSource(),
            new BitPayExchangeRateSource(),
            new BitfinexExchangeRateSource(),
            new CoinapultExchangeRateSource(),
            new ItBitExchangeRateSource()
        };

        private static IEnumerable<ExchangeRate> _exchangeRates = new ExchangeRate[] { };

        public static Task FetchRates(IEnumerable<RateDescriptor> rateDescriptors, Action<double> onProgress = null) => FetchSpecifiedRates(rateDescriptors, onProgress: onProgress);

        private static async Task FetchSpecifiedRates(IEnumerable<RateDescriptor> rateDescriptors, bool cleanDatabase = false, Action<double> onProgress = null)
        {
            var descriptorsNeededToFetch = rateDescriptors.SelectMany(RateHelper.GetNeededRatesForCalculation).Distinct().ToList();

            var currentLoop = 0;
            var fetchedRates = (await Task.WhenAll(Sources.Select(async source =>
            {
                var availableDescriptors = descriptorsNeededToFetch.Where(source.IsAvailable).ToList();
                descriptorsNeededToFetch = descriptorsNeededToFetch.Except(availableDescriptors).ToList();

                var rates = await source.FetchRates(availableDescriptors);

                currentLoop += 1;
                onProgress?.Invoke((double)currentLoop / Sources.Count());

                return rates;
            })))
            .SelectMany(rates => rates);

            await SaveRates(fetchedRates.ToList(), cleanDatabase);
        }

        public static Task FetchAllNeededRates(Action<double> onProgress = null)
            => FetchSpecifiedRates(RateHelper.NeededRates, onProgress: onProgress);

        public static Task FetchNotLoadedNeededRates(Action<double> onProgress = null)
        {
            var notLoadedNeededRates = RateHelper.NeededRates.Where(rateDescriptor => rateDescriptor.GetRate() == null);
            return FetchSpecifiedRates(notLoadedNeededRates, onProgress: onProgress);
        }

        public static Task FetchAllNeededRateFor(string currencyId, Action<double> onProgress = null)
        {
            var neededRates = ApplicationSettings.AllReferenceCurrencies.Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId));
            return FetchSpecifiedRates(neededRates, onProgress: onProgress);
        }

        public static async Task FetchAllFiatToCryptoRates(Action<double> onProgress = null)
        {
            var sources = Sources.Where(r => r.Type == RateSourceType.CryptoToFiat).ToList();
            var fetchedRates = new List<ExchangeRate>();

            var progress = .0;
            foreach (var source in sources)
            {
                var ratesToFetch = new[]
                {
                    new RateDescriptor(CurrencyConstants.Usd.Id, CurrencyConstants.Btc.Id),
                    new RateDescriptor(CurrencyConstants.Eur.Id, CurrencyConstants.Btc.Id)
                }.Where(rateDescriptor => source.IsAvailable(rateDescriptor)).ToList();

                var rates = await source.FetchRates(ratesToFetch);
                fetchedRates.AddRange(rates);

                progress += 1;
                onProgress?.Invoke(progress / sources.Count);
            }
            await SaveRates(fetchedRates);
        }

        private static async Task SaveRates(List<ExchangeRate> fetchedRates, bool cleanDatabase = false)
        {
            if (!LoadedFromDatabase)
            {
                await LoadFromDatabase();
            }
            await DatabaseUtil.Connection.RunInTransactionAsync(sqliteConnection =>
            {
                lock (SaveToDatabaseLock)
                {
                    var existingRates = _exchangeRates.ToList();

                    var newRates = fetchedRates.Except(existingRates).ToList();
                    var ratesToUpdate = fetchedRates.Intersect(existingRates).ToList();

                    var rates = existingRates.Except(ratesToUpdate).Concat(ratesToUpdate).Concat(newRates).ToList();
                    _exchangeRates = rates;

                    sqliteConnection.InsertAll(newRates.Select(rate => new ExchangeRateDbm(rate)));
                    sqliteConnection.UpdateAll(ratesToUpdate.Select(rate => new ExchangeRateDbm(rate)));

                    if (!cleanDatabase) return;

                    var ratesToDelete = existingRates.Where(rate => GetSourceFor(rate)?.Type != RateSourceType.CryptoToFiat).Except(fetchedRates);
                    foreach (var rate in ratesToDelete)
                    {
                        sqliteConnection.Delete(new ExchangeRateDbm(rate));
                    }
                }
            });
        }

        private static IRateSource GetSourceFor(ExchangeRate rate) => Sources.FirstOrDefault(source => (int)source.Id == rate.SourceId);

        public static async Task LoadFromDatabase()
        {
            await DatabaseUtil.Connection.CreateTableAsync<ExchangeRateDbm>();
            var allRates = (await DatabaseUtil.Connection.Table<ExchangeRateDbm>().ToListAsync()).Select(dbm => dbm.ExchangeRate);
            _exchangeRates = allRates;
            LoadedFromDatabase = true;
        }

        public static ExchangeRate GetRateOrDefault(RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new ExchangeRate(rateDescriptor, 1);

            bool RatesFilter(ExchangeRate rate)
            {
                if (rateDescriptor.HasCryptoAndFiatCurrency())
                    return rate.SourceId == SelectedCryptoToFiatSourceId && rate.RateDescriptor.CurrenciesEqual(rateDescriptor);

                return rate.RateDescriptor.CurrenciesEqual(rateDescriptor);
            }

            var result = _exchangeRates.FirstOrDefault(RatesFilter);
            if (result == null) return null;
            return result.RateDescriptor.Equals(rateDescriptor) ? result : result.Inverse();
        }

        public static IEnumerable<ExchangeRate> AllCryptoToFiateRates => _exchangeRates.Where(rate => GetSourceFor(rate).Type == RateSourceType.CryptoToFiat);

        internal static IRateSource SelectedCryptoToFiatSource =>
           Sources.FirstOrDefault(source => (int)source.Id == SelectedCryptoToFiatSourceId);

        private static int SelectedCryptoToFiatSourceId => ApplicationSettings.PreferredBitcoinRepository;
    }
}