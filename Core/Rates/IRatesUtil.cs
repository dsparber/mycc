using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates
{
    public interface IRatesUtil
    {
        ExchangeRate GetRate(RateDescriptor rateDescriptor);

        bool HasRate(RateDescriptor rateDescriptor);

        DateTime LastUpdate();
        DateTime LastUpdateFor(string currencyId);


        Task LoadFromDatabase();

        Task Fetch(IEnumerable<RateDescriptor> rateDescriptors, Action<double> onProgress = null);
        Task FetchNeeded(Action<double> onProgress = null);
        Task FetchNeededButNotLoaded(Action<double> onProgress = null);
        Task FetchFor(string currencyId, Action<double> onProgress = null);
        Task FetchAllFiatToCrypto(Action<double> onProgress = null);


        IEnumerable<(string name, IEnumerable<ExchangeRate> rates)> CryptoToFiatSourcesWithRates { get; }
    }
}