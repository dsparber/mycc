using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data
{
    public static class TaskHelper
    {
        public static async void UpdateRates()
        {
            await ApplicationTasks.FetchRates();
            await FetchMissingRates();
            Messaging.Update.Rates.Send();
        }

        public static async void UpdateAssets()
        {
            await ApplicationTasks.FetchAccounts();
            await ApplicationTasks.FetchRates();
            await FetchMissingRates(AccountStorage.NeededRates.ToList());
            Messaging.Update.Assets.Send();
        }

        public static async Task FetchMissingRates() // TODO Remove with new API --> FetchRates() should get all needed rates
        {
            var neededRates = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .SelectMany(c => ApplicationSettings.AllReferenceCurrencies.Select(r => new ExchangeRate(r, c)))
                .Distinct()
                .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                .Where(r => r.Rate == null)
                .Concat(AccountStorage.NeededRates).Distinct().ToList();
            await FetchMissingRates(neededRates);
        }

        private static async Task FetchMissingRates(List<ExchangeRate> neededRates)
        {
            if (neededRates.Count > 0)
            {
                await ApplicationTasks.FetchMissingRates(neededRates);
            }
        }
    }
}