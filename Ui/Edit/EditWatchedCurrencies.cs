using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;
using MyCC.Ui.Get;
using MyCC.Ui.Get.Implementations;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Edit
{
    internal static class EditWatchedCurrencies
    {
        public static void Remove(string currencyId)
        {
            if (ApplicationSettings.MainCurrencies.Contains(currencyId) || AccountStorage.UsedCurrencies.Contains(currencyId))
            {
                ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Concat(new[] { currencyId });
            }
            else if (ApplicationSettings.WatchedCurrencies.Contains(currencyId))
            {
                ApplicationSettings.WatchedCurrencies = ApplicationSettings.WatchedCurrencies.Except(new[] { currencyId }).ToList();
            }
            else if (ApplicationSettings.FurtherCurrencies.Contains(currencyId))
            {
                ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Except(new[] { currencyId }).ToList();
            }
            Messaging.UiUpdate.RatesOverview.Send();

        }

        public static void Add(string currencyId)
        {
            ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Except(new[] { currencyId });
            if (!UiUtils.Get.Rates.EnabledCurrencyIds.Contains(currencyId))
            {
                ApplicationSettings.WatchedCurrencies = new List<string>(ApplicationSettings.WatchedCurrencies) { currencyId };
            }
            UiUtils.Update.FetchNeededButNotLoadedRates();
            UiUtils.RatesRefresh.ResetCache();
            Messaging.UiUpdate.ViewsWithRate.Send();
        }
    }
}