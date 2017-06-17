using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Settings;

namespace MyCC.Core.Rates.Utils
{
    public class RatesUtil : IRatesUtil
    {
        public IEnumerable<(string name, IEnumerable<ExchangeRate> rates)> CryptoToFiatSourcesWithRates
            => RatesConfig.Sources.Where(source => source.Type == RateSourceType.CryptoToFiat).Select(source =>
             {
                 var rates = RateDatabase.CryptoToFiatRates.Where(rate => rate.SourceId == source.Id);
                 return (source.Name, rates);
             });


        public ExchangeRate GetRate(RateDescriptor rateDescriptor) => rateDescriptor.GetRate();
        public bool HasRate(RateDescriptor rateDescriptor) => rateDescriptor.GetRate() != null;

        public DateTime LastUpdate() => GetNeededRates().Select(e => e.GetRate()?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

        public DateTime LastUpdateFor(string currencyId)
            => GetNeededRatesFor(currencyId).Select(e => e.GetRate()?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

        public DateTime LastCryptoToFiatUpdate()
            => CryptoToFiatSourcesWithRates.Min(tuple => tuple.rates.Any() ? tuple.rates.Min(rate => rate.LastUpdate) : DateTime.MinValue);

        public Task LoadFromDatabase() => RateDatabase.LoadFromDatabase();

        public Task Fetch(IEnumerable<RateDescriptor> rateDescriptors, Action<double> onProgress = null) => RateLoader.FetchRates(rateDescriptors, onProgress: onProgress);

        public Task FetchNeeded(Action<double> onProgress = null)
            => RateLoader.FetchRates(GetNeededRates(), true, onProgress);

        public Task FetchNeededButNotLoaded(Action<double> onProgress = null)
            => RateLoader.FetchRates(GetNeededRates().Where(rateDescriptor => rateDescriptor.GetRate() == null), onProgress: onProgress);


        public Task FetchFor(string currencyId, Action<double> onProgress = null)
            => RateLoader.FetchRates(GetNeededRatesFor(currencyId), onProgress: onProgress);


        public Task FetchAllFiatToCrypto(Action<double> onProgress = null) =>
            RateLoader.FetchAllFiatToCryptoRates(onProgress);

        public int CryptoToFiatSourceCount => RatesConfig.Sources.Count(source => source.Type == RateSourceType.CryptoToFiat);

        public string SelectedCryptoToFiatSource
        {
            get => RatesConfig.SelectedCryptoToFiatSource.Name;
            set => RatesConfig.SelectedCryptoToFiatSourceName = value;
        }

        private static IEnumerable<RateDescriptor> GetNeededRates()
        {
            var referenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToList();
            var usedCurrencies = AccountStorage.UsedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(ApplicationSettings.WatchedCurrencies);

            return usedCurrencies.SelectMany(currencyId => referenceCurrencies.Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId))).Distinct();
        }

        private static IEnumerable<RateDescriptor> GetNeededRatesFor(string currencyId)
        {
            return ApplicationSettings.AllReferenceCurrencies
                .Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId));
        }
    }
}
