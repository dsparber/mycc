using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;

namespace MyCC.Ui.Get
{
    public static class CurrencySettingsData
    {
        public static void Disable(Currency currency)
        {
            if (ApplicationSettings.MainCurrencies.Contains(currency.Id) || AccountStorage.UsedCurrencies.Contains(currency.Id))
            {
                ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Concat(new[] { currency.Id });
            }
            else if (ApplicationSettings.WatchedCurrencies.Contains(currency.Id))
            {
                ApplicationSettings.WatchedCurrencies = ApplicationSettings.WatchedCurrencies.Except(new[] { currency.Id }).ToList();
            }
            else if (ApplicationSettings.FurtherCurrencies.Contains(currency.Id))
            {
                ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Except(new[] { currency.Id }).ToList();
            }
        }

        public static IEnumerable<Currency> EnabledCurrencies => AccountStorage.UsedCurrencies
            .Concat(ApplicationSettings.AllReferenceCurrencies)
            .Concat(ApplicationSettings.WatchedCurrencies)
            .Except(ApplicationSettings.DisabledCurrencyIds)
            .Distinct()
            .Where(c => c != null)
            .Select(CurrencyHelper.Find);

        public static void Add(Currency currency)
        {
            ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Except(new[] { currency.Id });
            if (!EnabledCurrencies.Contains(currency))
            {
                ApplicationSettings.WatchedCurrencies = new List<string>(ApplicationSettings.WatchedCurrencies) { currency.Id };
            }
        }
    }
}