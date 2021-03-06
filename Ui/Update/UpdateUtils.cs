using System;
using System.Threading.Tasks;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Tasks;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Ui.Update
{
    internal class UpdateUtils : IUpdateUtils
    {
        public async Task LoadNeededDataFromDatabaseAsync()
        {
            await ApplicationTasks.LoadEverything();
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.Update.Rates.Send();
            Messaging.Update.Balances.Send();
            Messaging.Update.CoinInfos.Send();
            Messaging.Update.CryptoToFiatRates.Send();
        }

        public void FetchAllRates() => ExecuteWithErrorWrapper(async () =>
       {
           await MyccUtil.Rates.FetchNeeded(Messaging.Status.Progress.Send);
           UiUtils.AssetsRefresh.ResetCache();
           UiUtils.RatesRefresh.ResetCache();
           Messaging.Update.Rates.Send();
           Messaging.Status.Progress.Send(1);
       });

        public void FetchCurrencies() => ExecuteWithErrorWrapper(async () =>
        {
            await CurrencyStorage.Instance.LoadOnline();
        });

        public void FetchAllAssetsAndRates() => ExecuteWithErrorWrapper(async () =>
       {
           Messaging.Status.Progress.Send(0.1);
           await ApplicationTasks.FetchAccounts(onError: e => throw e, progressCallback: progress => Messaging.Status.Progress.Send(0.1 + progress * 0.6));
           await MyccUtil.Rates.FetchNeeded(progress => Messaging.Status.Progress.Send(0.7 + progress * 0.3));
           UiUtils.AssetsRefresh.ResetCache();
           UiUtils.RatesRefresh.ResetCache();
           Messaging.Update.Rates.Send();
           Messaging.Update.Balances.Send();
           Messaging.Status.Progress.Send(1);
       });

        public void FetchNeededButNotLoadedRates() => ExecuteWithErrorWrapper(async () =>
       {
           await MyccUtil.Rates.FetchNeededButNotLoaded(Messaging.Status.Progress.Send);
           UiUtils.AssetsRefresh.ResetCache();
           UiUtils.RatesRefresh.ResetCache();
           Messaging.Update.Rates.Send();
           Messaging.Status.Progress.Send(1);
       });

        public void FetchBalancesAndRatesFor(string currencyId) => ExecuteWithErrorWrapper(async () =>
       {
           Messaging.Status.Progress.Send(0.2);
           await ApplicationTasks.FetchBalance(currencyId.Find(), onError: e => throw e, progressCallback: d => Messaging.Status.Progress.Send(0.2 + 0.4 * d));
           await MyccUtil.Rates.FetchFor(currencyId, progress => Messaging.Status.Progress.Send(0.6 + 0.4 * progress));
           UiUtils.AssetsRefresh.ResetCache();
           UiUtils.RatesRefresh.ResetCache();
           Messaging.Update.Rates.Send();
           Messaging.Update.Balances.Send();
           Messaging.Status.Progress.Send(1);
       });

        public void FetchBalanceAndRatesFor(int accountId) => ExecuteWithErrorWrapper(async () =>
        {
            var account = AccountStorage.GetAccount(accountId) as FunctionalAccount;
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchBalance(account, onError: e => throw e, onFinished: () => Messaging.Status.Progress.Send(0.2 + 0.4));
            await MyccUtil.Rates.FetchFor(account?.Money.Currency.Id, progress => Messaging.Status.Progress.Send(0.6 + 0.4 * progress));
            UiUtils.AssetsRefresh.ResetCache();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.Update.Rates.Send();
            Messaging.Update.Balances.Send();
            Messaging.Status.Progress.Send(1);
        });

        public void FetchCoinInfoAndRateFor(string currencyId) => ExecuteWithErrorWrapper(async () =>
       {
           Messaging.Status.Progress.Send(0.2);
           await ApplicationTasks.FetchCoinInfo(currencyId, onError: e => throw e, onFinished: () => Messaging.Status.Progress.Send(0.2 + 0.4));
           await MyccUtil.Rates.FetchFor(currencyId, progress => Messaging.Status.Progress.Send(0.6 + 0.4 * progress));
           Messaging.Update.CoinInfos.Send();
           Messaging.Status.Progress.Send(1);
       });

        public void FetchCoinInfoFor(string currencyId) => ExecuteWithErrorWrapper(async () =>
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchCoinInfo(currencyId, onError: e => throw e, onFinished: () => Messaging.Status.Progress.Send(0.9));
            Messaging.Update.CoinInfos.Send();
            Messaging.Status.Progress.Send(1);
        });

        public void FetchCryptoToFiatRates() => ExecuteWithErrorWrapper(async () =>
       {
           await MyccUtil.Rates.FetchAllFiatToCrypto(progess => Messaging.Status.Progress.Send(progess));
           Messaging.Update.CryptoToFiatRates.Send();
           Messaging.Update.Rates.Send();
       });

        private static async void ExecuteWithErrorWrapper(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                Messaging.Status.Progress.Send(1);
                DependencyService.Get<IErrorDialog>().Display(e);
            }
        }

        public void ConnectivityChanged(bool connected)
        {
            Messaging.Status.Network.Send(connected);
        }
    }
}