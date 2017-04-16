using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Dialogs;

namespace MyCC.Ui.Android.Data.Get
{
    public static class TaskHelper
    {
        public static async void UpdateRates()
        {
            await ApplicationTasks.FetchRates();
            FetchMissingRates(false);
            Messaging.Update.Rates.Send();
        }

        public static async void UpdateAssets()
        {
            await ApplicationTasks.FetchAccounts(onError: ErrorDialog.Display);
            await ApplicationTasks.FetchRates(onError: ErrorDialog.Display);
            await FetchMissingRates(AccountStorage.NeededRates.ToList());
            Messaging.Update.Assets.Send();
        }

        public static async void FetchMissingRates(bool sendMessage = true) // TODO Remove with new API --> FetchRates() should get all needed rates
        {
            var neededRates = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .SelectMany(c => ApplicationSettings.AllReferenceCurrencies.Select(r => new ExchangeRate(r, c)))
                .Distinct()
                .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                .Where(r => r.Rate == null)
                .Concat(AccountStorage.NeededRates).Distinct().ToList();
            await FetchMissingRates(neededRates);
            if (!sendMessage) return;
            Messaging.Update.Assets.Send();
            Messaging.Update.Rates.Send();
        }

        private static async Task FetchMissingRates(IReadOnlyCollection<ExchangeRate> neededRates)
        {
            if (neededRates.Count > 0)
            {
                await ApplicationTasks.FetchMissingRates(neededRates, onError: ErrorDialog.Display);
            }
        }
    }
}