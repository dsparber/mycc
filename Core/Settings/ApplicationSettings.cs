using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Types;

namespace MyCC.Core.Settings
{
    public static class ApplicationSettings
    {
        private static CoreVersion _lastCoreVersion;

        public static CoreVersion LastCoreVersion
        {
            get
            {
                if (_lastCoreVersion != null) return _lastCoreVersion;

                if (FirstLaunch)
                {
                    SettingKeys.KeyAppVersion.Set(Constants.CoreVersion.ToString());
                    _lastCoreVersion = Constants.CoreVersion;
                    return _lastCoreVersion;
                }

                var persitedValue = new CoreVersion(SettingKeys.KeyAppVersion.Get("0.0.0"));
                SettingKeys.KeyAppVersion.Set(Constants.CoreVersion.ToString());

                _lastCoreVersion = persitedValue;
                return _lastCoreVersion;
            }
        }

        private static bool? _firstLaunch;

        public static bool FirstLaunch
        {
            get
            {
                if (_firstLaunch.HasValue) return _firstLaunch.Value;

                var persitedValue = SettingKeys.KeyFirstLaunch.Get(true);
                if (persitedValue)
                {
                    SettingKeys.KeyFirstLaunch.Set(false);
                }
                _firstLaunch = persitedValue;
                return _firstLaunch.Value;
            }
        }

        public static bool DataLoaded { get; set; }

        public static string StartupCurrencyAssets
        {
            get { return SettingKeys.KeyAssetsPageCurrency.Get(CurrencyConstants.Btc.Id); }
            set { SettingKeys.KeyAssetsPageCurrency.Set(value); }
        }

        public static string StartupCurrencyRates
        {
            get { return SettingKeys.KeyRatePageCurrency.Get(CurrencyConstants.Btc.Id); }
            set { SettingKeys.KeyRatePageCurrency.Set(value); }
        }

        public static IEnumerable<string> WatchedCurrencies
        {
            get { return SettingKeys.KeyWatchedCurrencies.GetStrings(); }
            set { SettingKeys.KeyWatchedCurrencies.SetStrings(value); }
        }

        public static IEnumerable<string> DisabledCurrencyIds
        {
            get { return SettingKeys.KeyDisabledCurrencies.GetStrings(); }
            set { SettingKeys.KeyDisabledCurrencies.SetStrings(value); }
        }


        public static string Pin
        {
            set
            {
                PinLength = string.IsNullOrEmpty(value) ? -1 : value.Length;
                SettingKeys.KeyPin.Set(string.IsNullOrWhiteSpace(value) ? string.Empty : value.Hash());
            }
        }

        public static bool IsPinValid(string pin) => SettingKeys.KeyPin.Get(string.Empty).Equals(pin.Hash());

        public static IEnumerable<string> MainCurrencies
        {
            get { return SettingKeys.KeyMainCurrencies.GetStrings(new[] { CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id, CurrencyConstants.Usd.Id }); }
            set { SettingKeys.KeyMainCurrencies.SetStrings(value); }
        }

        public static IEnumerable<string> FurtherCurrencies
        {
            get { return SettingKeys.KeyFurtherCurrencies.GetStrings(); }
            set { SettingKeys.KeyFurtherCurrencies.SetStrings(value); }
        }

        public static IEnumerable<string> AllReferenceCurrencies => MainCurrencies.Concat(FurtherCurrencies).Distinct().ToList();

        public static SortOrder SortOrderAssets
        {
            get { return SettingKeys.KeySortOrderTable.GetEnum(SortOrder.Alphabetical); }
            set { SettingKeys.KeySortOrderTable.SetEnum(value); }
        }

        public static SortDirection SortDirectionAssets
        {
            get { return SettingKeys.KeySortDirectionTable.GetEnum(SortDirection.Ascending); }
            set { SettingKeys.KeySortDirectionTable.SetEnum(value); }
        }

        public static SortOrder SortOrderRates
        {
            get { return SettingKeys.KeySortOrderRates.GetEnum(SortOrder.Alphabetical); }
            set { SettingKeys.KeySortOrderRates.SetEnum(value); }
        }

        public static SortDirection SortDirectionRates
        {
            get { return SettingKeys.KeySortDirectionRates.GetEnum(SortDirection.Ascending); }
            set { SettingKeys.KeySortDirectionRates.SetEnum(value); }
        }

        public static SortOrder SortOrderAccounts
        {
            get { return SettingKeys.KeySortOrderAccounts.GetEnum(SortOrder.Alphabetical); }
            set { SettingKeys.KeySortOrderAccounts.SetEnum(value); }
        }

        public static SortDirection SortDirectionAccounts
        {
            get { return SettingKeys.KeySortDirectionAccounts.GetEnum(SortDirection.Ascending); }
            set { SettingKeys.KeySortDirectionAccounts.SetEnum(value); }
        }

        public static SortOrder SortOrderReferenceValues
        {
            get { return SettingKeys.KeySortOrderReferenceValues.GetEnum(SortOrder.Alphabetical); }
            set { SettingKeys.KeySortOrderReferenceValues.SetEnum(value); }
        }

        public static SortDirection SortDirectionReferenceValues
        {
            get { return SettingKeys.KeySortDirectionReferenceValues.GetEnum(SortDirection.Ascending); }
            set { SettingKeys.KeySortDirectionReferenceValues.SetEnum(value); }
        }

        public static bool AutoRefreshOnStartup
        {
            get { return SettingKeys.KeyAutoRefreshOnStartup.Get(true); }
            set { SettingKeys.KeyAutoRefreshOnStartup.Set(value); }
        }

        public static bool AppInitialised
        {
            get { return SettingKeys.KeyAppInitialised.Get(false); }
            set { SettingKeys.KeyAppInitialised.Set(value); }
        }

        public static bool RoundMoney => false;

        public static int PreferredBitcoinRepository
        {
            get { return SettingKeys.PreferredBitcoinRepository.Get((int)RatesRepositories.Kraken); }
            set { SettingKeys.PreferredBitcoinRepository.Set(value); }
        }

        public static StartupPage DefaultStartupPage
        {
            get { return SettingKeys.KeyDefaultPage.GetEnum(StartupPage.RatesView); }
            set { SettingKeys.KeyDefaultPage.SetEnum(value); }
        }

        public static ColumnToHide AssetsColumToHideIfSmall
        {
            get { return SettingKeys.KeyAssetsColumnHideWhenSmall.GetEnum(ColumnToHide.None); }
            set { SettingKeys.KeyAssetsColumnHideWhenSmall.SetEnum(value); }
        }


        public static int PinLength
        {
            get { return SettingKeys.KeyPinLength.Get(-1); }
            private set { SettingKeys.KeyPinLength.Set(value); }
        }

        public static bool IsPinSet => PinLength != -1;

        public static bool IsFingerprintEnabled
        {
            get { return SettingKeys.KeyFingerprintSet.Get(false); }
            set { SettingKeys.KeyFingerprintSet.Set(value); }
        }
        public static bool LockByShaking
        {
            get { return SettingKeys.KeyLockByShaking.Get(false); }
            set { SettingKeys.KeyLockByShaking.Set(value); }
        }



    }
}