using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates
{
    public interface IRatesUtil
    {
        ExchangeRate GetRate(RateDescriptor rateDescriptor);

        Task LoadFromDatabase();

        Task FetchRates(IEnumerable<RateDescriptor> rateDescriptors, Action<double> onProgress = null);
        Task FetchAllNeededRates(Action<double> onProgress = null);
        Task FetchNotLoadedNeededRates(Action<double> onProgress = null);
        Task FetchAllNeededRateFor(string currencyId, Action<double> onProgress = null);
        Task FetchAllFiatToCryptoRates(Action<double> onProgress = null);

        IEnumerable<(string name, IEnumerable<ExchangeRate> rates)> CryptoToFiatSourcesWithRates { get; }
    }
}