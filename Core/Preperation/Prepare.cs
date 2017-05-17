using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using Version = MyCC.Core.Settings.Version;

namespace MyCC.Core.Preperation
{
    public static class Prepare
    {
        public static bool PreparingNeeded => ApplicationSettings.FirstLaunch || ApplicationSettings.LastCoreVersion < new Version(1, 1, 2);

        public static void ExecutePreperations()
        {
            if (ApplicationSettings.FirstLaunch || ApplicationSettings.LastCoreVersion < new Version(1, 0, 2))
            {
                ApplicationSettings.DefaultStartupPage = StartupPage.TableView;
            }
            if (ApplicationSettings.LastCoreVersion < new Version(1, 1, 1))
            {
                // Reset values
                ApplicationSettings.MainCurrencies = ApplicationSettings.TryToLoadOldCurrencies(Settings.Settings.KeyMainCurrencies) ?? new[] { CurrencyConstants.Eur, CurrencyConstants.Btc, CurrencyConstants.Usd }.Select(c => c.Id);
                ApplicationSettings.FurtherCurrencies = ApplicationSettings.TryToLoadOldCurrencies(Settings.Settings.KeyFurtherCurrencies) ?? new string[] { };
                ApplicationSettings.WatchedCurrencies = ApplicationSettings.TryToLoadOldCurrencies(Settings.Settings.KeyWatchedCurrencies) ?? new string[] { };
                ApplicationSettings.StartupCurrencyAssets = CurrencyConstants.Btc.Id;
                ApplicationSettings.StartupCurrencyRates = CurrencyConstants.Btc.Id;
                ApplicationSettings.DefaultStartupPage = StartupPage.RatesView;
                ApplicationSettings.SortDirectionAccounts = SortDirection.Ascending;
                ApplicationSettings.SortDirectionRates = SortDirection.Ascending;
                ApplicationSettings.SortDirectionAssets = SortDirection.Ascending;
                ApplicationSettings.SortDirectionReferenceValues = SortDirection.Ascending;
                ApplicationSettings.SortOrderRates = SortOrder.Alphabetical;
                ApplicationSettings.SortOrderAccounts = SortOrder.Alphabetical;
                ApplicationSettings.SortOrderAssets = SortOrder.Alphabetical;
                ApplicationSettings.SortOrderReferenceValues = SortOrder.Alphabetical;
            }
            if (ApplicationSettings.LastCoreVersion < new Version(1, 1, 2))
            {
                ApplicationSettings.AssetsColumToHideIfSmall = ColumnToHide.None;
            }
        }

        public static Task AsyncExecutePreperations => null;
    }
}