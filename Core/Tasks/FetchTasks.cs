using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies.Models;

namespace MyCC.Core.Tasks
{
    public static partial class ApplicationTasks
    {
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

        public static async Task FetchCoinInfo(string currencyId, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
        {
            try
            {
                onStarted?.Invoke();
                await CoinInfoStorage.Instance.FetchInfo(currencyId);
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