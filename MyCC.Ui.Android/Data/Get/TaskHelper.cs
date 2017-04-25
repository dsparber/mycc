using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Dialogs;

namespace MyCC.Ui.Android.Data.Get
{
    public static class TaskHelper
    {

        public static async void UpdateAllRates()
        {
            Messaging.Status.Progress.Send(0.2);
            FetchMissingRates(false, d => Messaging.Status.Progress.Send(0.2 + d * 0.2));
            await ApplicationTasks.FetchRates(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.4 + d * 0.6));
            Messaging.Update.Rates.Send();
            Messaging.Update.Assets.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void UpdateAllAssetsAndRates()
        {
            Messaging.Status.Progress.Send(0.1);
            await FetchMissingRates(AccountStorage.NeededRates.ToList(), d => Messaging.Status.Progress.Send(0.1 + d * 0.2));
            await ApplicationTasks.FetchAccounts(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.3 + d * 0.4));
            await ApplicationTasks.FetchRates(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.7 + d * 0.3));
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void UpdateDataForNewAccount()
        {
            Messaging.Status.Progress.Send(0.4);
            await FetchMissingRates(AccountStorage.NeededRates.ToList(), d => Messaging.Status.Progress.Send(0.4 + d * 0.6));
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void FetchMissingRates(bool sendMessage = true, Action<double> progessCallback = null) // TODO Remove with new API --> FetchRates() should get all needed rates
        {
            var neededRates = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .SelectMany(c => ApplicationSettings.AllReferenceCurrencies.Select(r => new ExchangeRate(r, c)))
                .Distinct()
                .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                .Where(r => r.Rate == null)
                .Concat(AccountStorage.NeededRates).Distinct().ToList();

            await FetchMissingRates(neededRates, progessCallback);

            if (!sendMessage) return;
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
        }

        public static async void UpdateBalancesAndRatesForCurrency(Currency currency)
        {
            Messaging.Status.Progress.Send(0.2);
            await FetchMissingRates(AccountStorage.NeededRatesFor(currency), d => Messaging.Status.Progress.Send(0.2 + 0.2 * d));
            await ApplicationTasks.FetchRates(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.4 + 0.3 * d));
            await ApplicationTasks.FetchBalance(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.7 + 0.3 * d));
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void UpdateBalanceAndRatesForAccount(FunctionalAccount account)
        {
            Messaging.Status.Progress.Send(0.2);
            await FetchMissingRates(AccountStorage.NeededRatesFor(account.Money.Currency), d => Messaging.Status.Progress.Send(0.2 + 0.3 * d));
            await ApplicationTasks.FetchRates(account.Money.Currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.5 + 0.4 * d));
            await ApplicationTasks.FetchBalance(account, onError: ErrorDialog.Display, onFinished: () => Messaging.Status.Progress.Send(1));
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void FetchCoinInfoAndRates(Currency currency)
        {
            Messaging.Status.Progress.Send(0.2);
            await FetchMissingRates(AccountStorage.NeededRatesFor(currency), d => Messaging.Status.Progress.Send(0.2 + 0.3 * d));
            await ApplicationTasks.FetchRates(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.5 + 0.4 * d));
            await ApplicationTasks.FetchCoinInfo(currency, onError: ErrorDialog.Display, onFinished: () => Messaging.Status.Progress.Send(1));
            Messaging.UiUpdate.CoinInfo.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void FetchCoinInfo(Currency currency)
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchCoinInfo(currency, onError: ErrorDialog.Display, onFinished: () => Messaging.Status.Progress.Send(0.9));
            Messaging.UiUpdate.CoinInfo.Send();
            Messaging.Status.Progress.Send(1);
        }

        private static async Task FetchMissingRates(IReadOnlyCollection<ExchangeRate> neededRates, Action<double> progessCallback = null)
        {
            if (neededRates.Count > 0)
            {
                await ApplicationTasks.FetchMissingRates(neededRates, onError: ErrorDialog.Display, progressCallback: d => progessCallback?.Invoke(d));
            }
        }

    }
}