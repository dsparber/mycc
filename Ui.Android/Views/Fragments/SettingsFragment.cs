using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V7.Preferences;
using MyCC.Core;
using MyCC.Core.Currencies;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        private Preference _securityPreference;
        private Preference _preferredBitcoinPreference;
        private Preference _referenceCurrenciesPreference;

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);

            var startupViewPreference = (ListPreference)FindPreference("default-page");
            var assetsToHideColumnPreference = (ListPreference)FindPreference("pref_assets_column_to_hide_if_small");
            _securityPreference = FindPreference("pref_security");
            _preferredBitcoinPreference = FindPreference("pref_preferred_bitcoin");
            _referenceCurrenciesPreference = FindPreference("pref_reference_currencies");

            var startupEntries = new[]
            {
                Resources.GetString(Resource.String.Rates),
                $"{Resources.GetString(Resource.String.Assets)} ({Resources.GetString(Resource.String.Table)})",
                $"{Resources.GetString(Resource.String.Assets)} ({Resources.GetString(Resource.String.Graph)})"
            };
            var startupValues = new[]
            {
                StartupPage.RatesView.ToString(),
                StartupPage.TableView.ToString(),
                StartupPage.GraphView.ToString(),
            };
            var startupDefault = ApplicationSettings.DefaultStartupPage.ToString();

            startupViewPreference.SetEntries(startupEntries);
            startupViewPreference.SetEntryValues(startupValues);
            startupViewPreference.SetDefaultValue(startupDefault);
            startupViewPreference.Summary = startupEntries[startupValues.ToList().IndexOf(startupDefault)];
            startupViewPreference.PreferenceChange += (sender, args) =>
            {
                startupDefault = args.NewValue.ToString();
                startupViewPreference.Summary = startupEntries[startupValues.ToList().IndexOf(startupDefault)];
            };

            var assetsToHideEntries = new[]
            {
                Resources.GetString(Resource.String.None),
                Resources.GetString(Resource.String.Amount),
                Resources.GetString(Resource.String.ReferenceValue),
            };
            var assetsToHideValues = new[]
            {
                ColumnToHide.None.ToString(),
                ColumnToHide.Amount.ToString(),
                ColumnToHide.Value.ToString(),
            };

            assetsToHideColumnPreference.SetEntries(assetsToHideEntries);
            assetsToHideColumnPreference.SetEntryValues(assetsToHideValues);
            assetsToHideColumnPreference.PreferenceChange += (sender, args) =>
            {
                SettingUtils.ClearCache();
            };

            _securityPreference.PreferenceClick += (sender, args) =>
            {
                StartActivity(new Intent(Context, typeof(SecurityActivity)));
            };

            _referenceCurrenciesPreference.PreferenceClick += (sender, args) => StartActivity(new Intent(Context, typeof(ReferenceCurrencyActivity)));
            _preferredBitcoinPreference.PreferenceClick += (sender, args) => StartActivity(new Intent(Context, typeof(PreferredBitcoinExchangeActivity)));


            FindPreference("pref_about").PreferenceClick += (sender, args) =>
            {
                var intent = new Intent(Context, typeof(WebviewActivity));
                intent.PutExtra(WebviewActivity.ExtraUrl, "file:///android_asset/about.html");
                intent.PutExtra(WebviewActivity.ExtraOpenLinksInNewActivity, true);
                intent.PutExtra(WebviewActivity.ExtraShowVersionHeader, true);
                intent.PutExtra(WebviewActivity.ExtraTitle, Resources.GetString(Resource.String.About));
                StartActivity(intent);
            };

            FindPreference("pref_privacy").PreferenceClick += (sender, args) =>
            {
                if (ConnectivityStatus.IsConnected)
                {
                    var intent = new Intent(Context, typeof(WebviewActivity));
                    intent.PutExtra(WebviewActivity.ExtraUrl, $"https://www.iubenda.com/privacy-policy/{Resources.GetString(Resource.String.PrivacyLinkId)}/full-legal");
                    intent.PutExtra(WebviewActivity.ExtraOpenLinksInNewActivity, true);
                    intent.PutExtra(WebviewActivity.ExtraShowVersionHeader, true);
                    intent.PutExtra(WebviewActivity.ExtraTitle, Resources.GetString(Resource.String.Privacy));
                    StartActivity(intent);
                }
                else
                {
                    Activity?.ShowInfoDialog(Resource.String.Error, Resource.String.NoInternetAccess);
                }

            };
        }

        public override void OnResume()
        {
            base.OnResume();

            _securityPreference.Summary = Resources.GetString(ApplicationSettings.IsPinSet && ApplicationSettings.IsFingerprintEnabled ? Resource.String.FingerprintActive : ApplicationSettings.IsPinSet ? Resource.String.PinActive : Resource.String.NotConfigured);
            _preferredBitcoinPreference.Summary = MyccUtil.Rates.SelectedCryptoToFiatSource;
            _referenceCurrenciesPreference.Summary = string.Join(", ", ApplicationSettings.AllReferenceCurrencies.Select(c => c.ToCurrency().Code));
        }
    }
}