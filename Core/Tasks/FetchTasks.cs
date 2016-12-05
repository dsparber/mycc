using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Storage;
using MyCryptos.Core.Types;

namespace MyCryptos.Core.tasks
{
    public static partial class ApplicationTasks
    {
        private static Task fetchAllExchangeRatesTask;
        private static Task fetchMissingRatesTask;
        private static Task fetchCurrenciesAndAvailableRatesTask;

        public static Task FetchBalancesAndExchangeRates(Action whenFinished, Action<Exception> onError)
        => fetchAllExchangeRatesTask = fetchAllExchangeRatesTask.GetTask(async () =>
        {
            try
            {
                await ExchangeRateStorage.Instance.Fetch();
                await AccountStorage.Instance.Fetch();
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                whenFinished();
            }
        });

        public static Task FetchCurrenciesAndAvailableRates(Action onFinished, Action<Exception> onError)
        => fetchCurrenciesAndAvailableRatesTask = fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
        {
            try
            {
                await CurrencyRepositoryMapStorage.Instance.Fetch();
                await CurrencyStorage.Instance.Fetch();
                await AvailableRatesStorage.Instance.Fetch();
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        });

        public static Task FetchMissingRates(IEnumerable<ExchangeRate.Model.ExchangeRate> rates, Action onFinished, Action<Exception> onError)
        => fetchMissingRatesTask = fetchMissingRatesTask.GetTask(async () =>
        {
            var ratesList = rates.ToList();
            ratesList.RemoveAll(e => e == null);
            if (ratesList.Count == 0) return;
            try
            {
                await Task.WhenAll(ratesList.Select(r => ExchangeRateHelper.GetRate(r, FetchSpeedEnum.Fast)));
                await ExchangeRateStorage.Instance.FetchNew();
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        });
    }
}