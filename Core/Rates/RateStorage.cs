using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Implementations;
using MyCC.Core.Settings;
using Xamarin.Forms;

namespace MyCC.Core.Rates
{
    public static class RateStorage
    {
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

        private static readonly IEnumerable<ExchangeRate> ExchangeRates = new ExchangeRate[] { };


        public static async Task FetchRates(IEnumerable<RateDescriptor> rateDescriptors)
        {
            // TODO Implement
        }

        public static async Task FetchAllFiatToCryptoRates(Action<double> onProgress = null)
        {
            var sources = Sources.Where(r => r.Type == RateSourceType.CryptoToFiat).ToList();

            var progress = .0;
            foreach (var source in sources)
            {
                var ratesToFetch = new[]
                {
                    new RateDescriptor(CurrencyConstants.Eur.Id, CurrencyConstants.Btc.Id),
                    new RateDescriptor(CurrencyConstants.Eur.Id, CurrencyConstants.Btc.Id)
                }.Where(rateDescriptor => source.IsAvailable(rateDescriptor));

                var rates = await source.FetchRates(ratesToFetch);

                progress += 1;
                onProgress?.Invoke(progress / sources.Count);
            }
        }

        private static async Task SaveRates()
        {
            // TODO Implement (thread save)
        }

        private static async Task LoadFromDb()
        {
            // TODO Implement
        }

        public static ExchangeRate GetRateOrDefault(RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new ExchangeRate(rateDescriptor, 1);

            bool RatesFilter(ExchangeRate rate)
            {
                if (rateDescriptor.HasCryptoAndFiatCurrency())
                    return rate.SourceId == SelectedCryptoToFiatSourceId && rate.RateDescriptor.Equals(rateDescriptor);

                return rate.RateDescriptor.Equals(rateDescriptor);
            }

            return ExchangeRates.FirstOrDefault(RatesFilter);
        }

        public static IRateSource SelectedCryptoToFiatSource =>
           Sources.FirstOrDefault(source => (int)source.Id == SelectedCryptoToFiatSourceId);

        private static int SelectedCryptoToFiatSourceId => ApplicationSettings.PreferredBitcoinRepository;
    }
}