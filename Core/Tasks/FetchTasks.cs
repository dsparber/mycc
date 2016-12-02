using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Models;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Helpers;

namespace MyCryptos.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        private static Task fetchAllExchangeRatesTask;
        private static Task fetchMissingRatesTask;
        private static Task fetchCurrenciesAndAvailableRatesTask;

        public static Task FetchAllExchangeRates()
        => fetchAllExchangeRatesTask = fetchAllExchangeRatesTask.GetTask(ExchangeRateStorage.Instance.FetchNew());

        public static Task FetchCurrenciesAndAvailableRates()
        => fetchCurrenciesAndAvailableRatesTask = fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
        {
            await CurrencyRepositoryMapStorage.Instance.Fetch();
            await CurrencyStorage.Instance.Fetch();
            await AvailableRatesStorage.Instance.Fetch();
        });

        public static Task FetchMissingRates(IEnumerable<ExchangeRate> rates) =>
            fetchMissingRatesTask = fetchMissingRatesTask.GetTask(async () =>
            {
                var ratesList = rates.ToList();
                ratesList.RemoveAll(e => e == null);
                if (ratesList.Count == 0) return;

                await Task.WhenAll(ratesList.Select(r => ExchangeRateHelper.GetRate(r, FetchSpeedEnum.FAST)));
                await ExchangeRateStorage.Instance.FetchNew();
            });
    }
}