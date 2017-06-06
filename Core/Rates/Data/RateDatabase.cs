using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Database;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Rates.Utils;

namespace MyCC.Core.Rates.Data
{
    internal static class RateDatabase
    {
        private static IEnumerable<ExchangeRate> _exchangeRates = new ExchangeRate[] { };
        private static readonly object SaveToDatabaseLock = new object();
        private static bool _loadedFromDatabase;

        public static IEnumerable<ExchangeRate> CryptoToFiatRates => _exchangeRates.Where(rate => GetSourceFor(rate).Type == RateSourceType.CryptoToFiat);

        public static async Task LoadFromDatabase()
        {
            await DatabaseUtil.Connection.CreateTableAsync<ExchangeRateDbm>();
            var allRates = (await DatabaseUtil.Connection.Table<ExchangeRateDbm>().ToListAsync()).Select(dbm => dbm.AsExchangeRate).DistinctCurrencyPairs().ToList();
            _exchangeRates = allRates;
            _loadedFromDatabase = true;
        }

        public static async Task SaveRates(List<ExchangeRate> fetchedRates, bool cleanDatabase = false)
        {
            if (!_loadedFromDatabase)
            {
                await LoadFromDatabase();
            }
            await DatabaseUtil.Connection.RunInTransactionAsync(sqliteConnection =>
            {
                lock (SaveToDatabaseLock)
                {
                    var existingRates = _exchangeRates.ToList();

                    var newRates = fetchedRates.Where(rate => !existingRates.Contains(rate.Descriptor)).ToList();
                    var ratesToUpdate = fetchedRates.Where(rate => existingRates.Contains(rate.Descriptor)).ToList();

                    var rates = existingRates.Where(rate => !ratesToUpdate.Contains(rate.Descriptor)).Concat(ratesToUpdate).Concat(newRates).ToList();
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

        private static IRateSource GetSourceFor(ExchangeRate rate) => RatesConfig.Sources.FirstOrDefault(source => source.Id == rate.SourceId);

        public static ExchangeRate GetRateOrDefault(RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new ExchangeRate(rateDescriptor, 1);

            bool RatesFilter(ExchangeRate rate)
            {
                if (rateDescriptor.HasCryptoAndFiatCurrency())
                    return rate.SourceId == RatesConfig.SelectedCryptoToFiatSourceId && rate.Descriptor.CurrenciesEqual(rateDescriptor);

                return rate.Descriptor.CurrenciesEqual(rateDescriptor);
            }

            var result = _exchangeRates.FirstOrDefault(RatesFilter);
            if (result == null) return null;
            return result.Descriptor.Equals(rateDescriptor) ? result : result.Inverse();
        }
    }
}