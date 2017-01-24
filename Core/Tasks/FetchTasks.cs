using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;

namespace MyCC.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        private static Task _fetchAllExchangeRatesTask;
        private static Task _fetchMissingRatesTask;
        private static Task _fetchCurrenciesAndAvailableRatesTask;

        public static Task FetchBalancesAndRates(Action onStarted, Action onFinished, Action<Exception> onError)
        => _fetchAllExchangeRatesTask = _fetchAllExchangeRatesTask.GetTask(async () =>
        {
            try
            {
                onStarted();
                await ExchangeRatesStorage.Instance.UpdateRates();
                await AccountStorage.Instance.FetchOnline();
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

        public static Task FetchCurrenciesAndAvailableRates(Action onStarted, Action onFinished, Action<Exception> onError)
        => _fetchCurrenciesAndAvailableRatesTask = _fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
        {
            try
            {
                onStarted();
                await CurrencyRepositoryMapStorage.Instance.FetchOnline();
                await CurrencyStorage.Instance.FetchOnline();
                await ExchangeRatesStorage.Instance.FetchAvailableRates();
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

        public static Task FetchMissingRates(IEnumerable<ExchangeRate> rates, Action onStarted, Action onFinished, Action<Exception> onError)
        => _fetchMissingRatesTask = _fetchMissingRatesTask.GetTask(async () =>
        {
            var ratesList = rates.ToList();
            ratesList.RemoveAll(e => e == null);
            if (ratesList.Count == 0) return;
            try
            {
                onStarted();
                await ExchangeRateHelper.FetchMissingRatesFor(ratesList);
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

        public static async Task FetchBalanceAndRates(OnlineFunctionalAccount account, Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await account.FetchBalanceOnline();
                var ratesToUpdate = ApplicationSettings.AllReferenceCurrencies.Select(c => new ExchangeRate(account.Money.Currency, c));
                await ExchangeRateHelper.UpdateRates(ratesToUpdate);
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }

        public static async Task FetchBalanceAndRates(Currency.Model.Currency currency, Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await Task.WhenAll(AccountStorage.Instance.AllElements.Where(a => a.Money.Currency.Equals(currency)).OfType<OnlineFunctionalAccount>().Select(a => a.FetchBalanceOnline()));
                var ratesToUpdate = ApplicationSettings.AllReferenceCurrencies.Select(c => new ExchangeRate(currency, c));
                await ExchangeRateHelper.UpdateRates(ratesToUpdate);
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }

        public static async Task FetchBalances(OnlineAccountRepository repository, Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await repository.FetchOnline();
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }

        public static async Task FetchAccounts(Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await AccountStorage.Instance.FetchOnline();
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }

        public static async Task FetchRates(List<ExchangeRate> neededRates, Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await ExchangeRateHelper.UpdateRates(neededRates);
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }

        public static async Task FetchCoinInfo(Currency.Model.Currency coin, Action onStarted, Action onFinished, Action<Exception> onError)
        {
            try
            {
                onStarted();
                await CoinInfoStorage.Instance.FetchInfo(coin);
            }
            catch (Exception e)
            {
                onError(e);
            }
            finally
            {
                onFinished();
            }
        }
    }
}