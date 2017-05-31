using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates;
using MyCC.Core.Settings;

namespace MyCC.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        public static async Task FetchRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                await ExchangeRateHelper.UpdateRates(progressCallback: progressCallback);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchCurrenciesAndAvailableRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
        {
            try
            {
                onStarted?.Invoke();
                await CurrencyStorage.Instance.LoadOnline();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchMissingRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                await ExchangeRateHelper.FetchMissingRates(progressCallback);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchBalance(FunctionalAccount account, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
        {
            try
            {
                onStarted?.Invoke();
                var onlineFunctionalAccount = account as OnlineFunctionalAccount;
                if (onlineFunctionalAccount != null) await onlineFunctionalAccount.FetchBalanceOnline();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static Task FetchRates(FunctionalAccount account, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
            => FetchRates(account.Money.Currency, onStarted, onFinished, onError, progressCallback);


        public static async Task FetchRates(Currency currency, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                var ratesToUpdate = ApplicationSettings.AllReferenceCurrencies.Select(c => new ExchangeRate(currency.Id, c));
                await ExchangeRateHelper.UpdateRates(ratesToUpdate, progressCallback);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchBalance(Currency currency, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                var accounts = AccountStorage.AccountsWithCurrency(currency).OfType<OnlineFunctionalAccount>().ToList();
                var progress = .0;
                await Task.WhenAll(accounts.Select(async a =>
                {
                    await a.FetchBalanceOnline();
                    progress += 1;
                    progressCallback?.Invoke(progress / accounts.Count);
                }));
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchAccounts(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                await AccountStorage.Instance.FetchOnline(progressCallback);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchRates(IEnumerable<ExchangeRate> neededRates, Action onStarted, Action onFinished, Action<Exception> onError)
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

        public static async Task FetchBitcoinDollarRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
        {
            try
            {
                onStarted?.Invoke();
                await ExchangeRateHelper.FetchDollarBitcoinRates(progressCallback);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        public static async Task FetchCoinInfo(Currency coin, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
        {
            try
            {
                onStarted?.Invoke();
                await CoinInfoStorage.Instance.FetchInfo(coin);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

    }
}