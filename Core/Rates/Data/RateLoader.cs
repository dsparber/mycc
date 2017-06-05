using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Rates.Utils;

namespace MyCC.Core.Rates.Data
{
    internal static class RateLoader
    {
        public static async Task FetchRates(IEnumerable<RateDescriptor> rateDescriptors, bool cleanDatabase = false, Action<double> onProgress = null)
        {
            var descriptorsNeededToFetch = rateDescriptors.SelectMany(RateCalculator.GetNeededRatesForCalculation).Distinct().ToList();

            var currentLoop = 0;
            var fetchedRates = (await Task.WhenAll(RatesConfig.Sources.Select(async source =>
            {
                var availableDescriptors = descriptorsNeededToFetch.Where(source.IsAvailable).ToList();
                descriptorsNeededToFetch = descriptorsNeededToFetch.Except(availableDescriptors).ToList();

                var rates = await source.FetchRates(availableDescriptors);

                currentLoop += 1;
                onProgress?.Invoke((double)currentLoop / RatesConfig.Sources.Count());

                return rates;
            })))
            .SelectMany(rates => rates);

            await RateDatabase.SaveRates(fetchedRates.ToList(), cleanDatabase);
        }

        public static async Task FetchAllFiatToCryptoRates(Action<double> onProgress = null)
        {
            var sources = RatesConfig.Sources.Where(r => r.Type == RateSourceType.CryptoToFiat).ToList();
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
            await RateDatabase.SaveRates(fetchedRates);
        }





    }
}