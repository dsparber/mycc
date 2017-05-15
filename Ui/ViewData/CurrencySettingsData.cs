using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Settings;

namespace MyCC.Ui.ViewData
{
    public static class CurrencySettingsData
    {
        public static void Disable(Currency currency)
        {
            if (ApplicationSettings.MainCurrencies.Contains(currency) || AccountStorage.UsedCurrencies.Contains(currency))
            {
                ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Concat(new[] { currency.Id });
            }
            else if (ApplicationSettings.WatchedCurrencies.Contains(currency))
            {
                ApplicationSettings.WatchedCurrencies = ApplicationSettings.WatchedCurrencies.Except(new[] { currency }).ToList();
            }
            else if (ApplicationSettings.FurtherCurrencies.Contains(currency))
            {
                ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Except(new[] { currency }).ToList();
            }
        }

        public static IEnumerable<Currency> EnabledCurrencies => AccountStorage.UsedCurrencies
            .Concat(ApplicationSettings.AllReferenceCurrencies)
            .Concat(ApplicationSettings.WatchedCurrencies)
            .Except(ApplicationSettings.DisabledCurrencyIds.Select(CurrencyStorage.Find))
            .Distinct();

        public static void Add(Currency currency)
        {
            ApplicationSettings.DisabledCurrencyIds = ApplicationSettings.DisabledCurrencyIds.Except(new[] { currency.Id });
            if (!EnabledCurrencies.Contains(currency))
            {
                ApplicationSettings.WatchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies) { currency };
            }
        }
    }
}