using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Sources.Utils;
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
            get => SettingKeys.KeyAssetsPageCurrency.Get(CurrencyConstants.Btc.Id);
            set => SettingKeys.KeyAssetsPageCurrency.Set(value);
        }

        public static string StartupCurrencyRates
        {
            get => SettingKeys.KeyRatePageCurrency.Get(CurrencyConstants.Btc.Id);
            set => SettingKeys.KeyRatePageCurrency.Set(value);
        }

        public static IEnumerable<string> WatchedCurrencies
        {
            get => SettingKeys.KeyWatchedCurrencies.GetStrings();
            set => SettingKeys.KeyWatchedCurrencies.SetStrings(value);
        }

        public static IEnumerable<string> DisabledCurrencyIds
        {
            get => SettingKeys.KeyDisabledCurrencies.GetStrings();
            set => SettingKeys.KeyDisabledCurrencies.SetStrings(value);
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
            get => SettingKeys.KeyMainCurrencies.GetStrings(SettingUtils.DefaultStaredReferenceCurrencies);
            set => SettingKeys.KeyMainCurrencies.SetStrings(value);
        }

        public static IEnumerable<string> FurtherCurrencies
        {
            get => SettingKeys.KeyFurtherCurrencies.GetStrings(SettingUtils.DefaultFurtherReferenceCurrencies);
            set => SettingKeys.KeyFurtherCurrencies.SetStrings(value);
        }

        public static IEnumerable<string> AllReferenceCurrencies
            => MainCurrencies.Concat(FurtherCurrencies).Distinct().OrderBy(id => id).ToList();

        public static SortOrder SortOrderAssets
        {
            get => SettingKeys.KeySortOrderTable.GetEnum(SortOrder.Alphabetical);
            set => SettingKeys.KeySortOrderTable.SetEnum(value);
        }

        public static SortDirection SortDirectionAssets
        {
            get => SettingKeys.KeySortDirectionTable.GetEnum(SortDirection.Ascending);
            set => SettingKeys.KeySortDirectionTable.SetEnum(value);
        }

        public static SortOrder SortOrderRates
        {
            get => SettingKeys.KeySortOrderRates.GetEnum(SortOrder.Alphabetical);
            set => SettingKeys.KeySortOrderRates.SetEnum(value);
        }

        public static SortDirection SortDirectionRates
        {
            get => SettingKeys.KeySortDirectionRates.GetEnum(SortDirection.Ascending);
            set => SettingKeys.KeySortDirectionRates.SetEnum(value);
        }

        public static SortOrder SortOrderAccounts
        {
            get => SettingKeys.KeySortOrderAccounts.GetEnum(SortOrder.Alphabetical);
            set => SettingKeys.KeySortOrderAccounts.SetEnum(value);
        }

        public static SortDirection SortDirectionAccounts
        {
            get => SettingKeys.KeySortDirectionAccounts.GetEnum(SortDirection.Ascending);
            set => SettingKeys.KeySortDirectionAccounts.SetEnum(value);
        }

        public static SortOrder SortOrderReferenceValues
        {
            get => SettingKeys.KeySortOrderReferenceValues.GetEnum(SortOrder.Alphabetical);
            set => SettingKeys.KeySortOrderReferenceValues.SetEnum(value);
        }

        public static SortDirection SortDirectionReferenceValues
        {
            get => SettingKeys.KeySortDirectionReferenceValues.GetEnum(SortDirection.Ascending);
            set => SettingKeys.KeySortDirectionReferenceValues.SetEnum(value);
        }

        public static bool AutoRefreshOnStartup
        {
            get => SettingKeys.KeyAutoRefreshOnStartup.Get(true);
            set => SettingKeys.KeyAutoRefreshOnStartup.Set(value);
        }

        public static bool AppInitialised
        {
            get => SettingKeys.KeyAppInitialised.Get(false);
            set => SettingKeys.KeyAppInitialised.Set(value);
        }

        public static bool RoundMoney => false;

        public static int PreferredBitcoinRepository
        {
            get => SettingKeys.PreferredBitcoinRepository.Get((int)RateSourceId.Kraken);
            set => SettingKeys.PreferredBitcoinRepository.Set(value);
        }

        public static StartupPage DefaultStartupPage
        {
            get => SettingKeys.KeyDefaultPage.GetEnum(StartupPage.RatesView);
            set => SettingKeys.KeyDefaultPage.SetEnum(value);
        }

        public static ColumnToHide AssetsColumToHideIfSmall
        {
            get => SettingKeys.KeyAssetsColumnHideWhenSmall.GetEnum(ColumnToHide.None);
            set => SettingKeys.KeyAssetsColumnHideWhenSmall.SetEnum(value);
        }


        public static int PinLength
        {
            get => SettingKeys.KeyPinLength.Get(-1);
            private set => SettingKeys.KeyPinLength.Set(value);
        }

        public static bool IsPinSet => PinLength != -1;

        public static bool IsFingerprintEnabled
        {
            get => SettingKeys.KeyFingerprintSet.Get(false);
            set => SettingKeys.KeyFingerprintSet.Set(value);
        }

        public static bool LockByShaking
        {
            get => SettingKeys.KeyLockByShaking.Get(false);
            set => SettingKeys.KeyLockByShaking.Set(value);
        }

        public static bool SecureXpub
        {
            get => !IsPinSet || SettingKeys.KeySecureXpub.Get(true);
            set => SettingKeys.KeySecureXpub.Set(value);
        }

        public static long PoloniexRequestNonce
        {
            get
            {
                try
                {
                    var nounce = long.Parse(SettingKeys.KeyPoloniexRequestNonce.Get("0"));
                    return PoloniexRequestNonce = nounce + 1;
                }
                catch
                {
                    return 0;
                }
            }
            set => SettingKeys.KeyPoloniexRequestNonce.Set(value.ToString());
        }
    }

}