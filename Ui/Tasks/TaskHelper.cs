using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using MyCC.Ui.ViewData;
using Xamarin.Forms;

namespace MyCC.Ui.Tasks
{
    public static class TaskHelper
    {

        private static IErrorDialog ErrorDialog => DependencyService.Get<IErrorDialog>();

        public static async void UpdateAllRates()
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchRates(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.2 + d * 0.6));
            Messaging.Update.Rates.Send();
            Messaging.Update.Assets.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async Task UpdateAllAssetsAndRates()
        {
            Messaging.Status.Progress.Send(0.1);
            await ApplicationTasks.FetchAccounts(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.1 + d * 0.6));
            await ApplicationTasks.FetchRates(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.7 + d * 0.3));
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void UpdateDataForNewAccount()
        {
            Messaging.Status.Progress.Send(0.4);
            await FetchMissing(d => Messaging.Status.Progress.Send(0.4 + d * 0.6));
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async Task FetchMissingRates(bool sendMessage = true, Action<double> progessCallback = null, Action onFinish = null) // TODO Remove with new API --> FetchRates() should get all needed rates
        {
            try
            {
                await FetchMissing(progessCallback);
                onFinish?.Invoke();

                if (!sendMessage) return;
                Messaging.Update.Assets.Send();
                Messaging.Update.Rates.Send();
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        public static async void UpdateBalancesAndRatesForCurrency(Currency currency)
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchRates(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.2 + 0.5 * d));
            await ApplicationTasks.FetchBalance(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.7 + 0.3 * d));
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void UpdateBalanceAndRatesForAccount(FunctionalAccount account)
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchRates(account.Money.Currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.2 + 0.7 * d));
            await ApplicationTasks.FetchBalance(account, onError: ErrorDialog.Display, onFinished: () => Messaging.Status.Progress.Send(1));
            Messaging.Update.Rates.Send();
            Messaging.Status.Progress.Send(1);
        }

        public static async void FetchCoinInfoAndRates(Currency currency)
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchRates(currency, onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.2 + 0.7 * d));
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

        public static async void UpdateBitcoinExchangeSources()
        {
            Messaging.Status.Progress.Send(0.2);
            await ApplicationTasks.FetchBitcoinDollarRates(onError: ErrorDialog.Display, progressCallback: d => Messaging.Status.Progress.Send(0.2 + 0.8 * d));

            Messaging.UiUpdate.BitcoinExchangeSources.Send();
            Messaging.Status.Progress.Send(1);
        }

        private static async Task FetchMissing(Action<double> progessCallback = null)
        {
            await ApplicationTasks.FetchMissingRates(onError: ErrorDialog.Display, progressCallback: d => progessCallback?.Invoke(d));
        }
    }
}