using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;

namespace MyCC.Core.Rates.Utils
{
    public class RatesUtil : IRatesUtil
    {
        public IEnumerable<(string name, IEnumerable<ExchangeRate> rates)> CryptoToFiatSourcesWithRates
            => RateDatabase.CryptoToFiatRates.GroupBy(rate => rate.SourceId).Select(group =>
            {
                var sourceName = RatesConfig.Sources.First(source => group.Key == source.Id).Name;
                var rates = group.ToList() as IEnumerable<ExchangeRate>;
                return (sourceName, rates);
            });

        public ExchangeRate GetRate(RateDescriptor rateDescriptor) => rateDescriptor.GetRate();

        public Task LoadFromDatabase() => RateDatabase.LoadFromDatabase();

        public Task FetchRates(IEnumerable<RateDescriptor> rateDescriptors, Action<double> onProgress = null) => RateLoader.FetchRates(rateDescriptors, onProgress: onProgress);

        public Task FetchAllNeededRates(Action<double> onProgress = null)
            => RateLoader.FetchRates(GetNeededRates(), onProgress: onProgress);

        public Task FetchNotLoadedNeededRates(Action<double> onProgress = null)
            => RateLoader.FetchRates(GetNeededRates().Where(rateDescriptor => rateDescriptor.GetRate() == null), onProgress: onProgress);


        public Task FetchAllNeededRateFor(string currencyId, Action<double> onProgress = null)
        {
            var neededRates = ApplicationSettings.AllReferenceCurrencies.Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId));
            return RateLoader.FetchRates(neededRates, onProgress: onProgress);
        }

        public Task FetchAllFiatToCryptoRates(Action<double> onProgress = null) =>
            RateLoader.FetchAllFiatToCryptoRates(onProgress);

        private static IEnumerable<RateDescriptor> GetNeededRates()
        {
            var referenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToList();
            var usedCurrencies = AccountStorage.UsedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(ApplicationSettings.WatchedCurrencies);

            return usedCurrencies.SelectMany(currencyId => referenceCurrencies.Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId))).Distinct();
        }

    }
}
