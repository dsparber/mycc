using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Settings;
using MyCC.Core.Types;

namespace MyCC.Core.Preperation
{
    public static class Prepare
    {
        public static bool PreparingNeeded => ApplicationSettings.FirstLaunch || ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1, 2);

        public static void ExecutePreperations()
        {
            if (ApplicationSettings.FirstLaunch)
            {
                ApplicationSettings.AssetsColumToHideIfSmall = ColumnToHide.None;
                ApplicationSettings.DefaultStartupPage = StartupPage.RatesView;
            }
            if (ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1, 1))
            {
                ApplicationSettings.MainCurrencies = SettingKeys.KeyMainCurrencies.TryToLoadOldCurrencies() ?? SettingUtils.DefaultStaredReferenceCurrencies;
                ApplicationSettings.FurtherCurrencies = SettingKeys.KeyFurtherCurrencies.TryToLoadOldCurrencies() ?? SettingUtils.DefaultFurtherReferenceCurrencies;
                ApplicationSettings.WatchedCurrencies = SettingKeys.KeyWatchedCurrencies.TryToLoadOldCurrencies() ?? new string[] { };
                ApplicationSettings.StartupCurrencyAssets = SettingKeys.KeyAssetsPageCurrency.TryToLoadOldCurrency() ?? CurrencyConstants.Btc.Id;
                ApplicationSettings.StartupCurrencyRates = SettingKeys.KeyRatePageCurrency.TryToLoadOldCurrency() ?? CurrencyConstants.Btc.Id;
                ApplicationSettings.DefaultStartupPage = SettingKeys.KeyDefaultPage.TryToLoadOldStartupPage();
            }
            if (ApplicationSettings.LastCoreVersion < new CoreVersion(1, 1, 2))
            {
                ApplicationSettings.AssetsColumToHideIfSmall = ColumnToHide.None;
            }
        }

        public static Task AsyncExecutePreperations => null;
    }
}