using Android.Content;
using Android.OS;
using Android.Support.V7.Preferences;
using MyCC.Ui.Android.Views.Activities;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);

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
                var intent = new Intent(Context, typeof(WebviewActivity));
                intent.PutExtra(WebviewActivity.ExtraUrl, $"https://www.iubenda.com/privacy-policy/{Resources.GetString(Resource.String.PrivacyLinkId)}/full-legal");
                intent.PutExtra(WebviewActivity.ExtraOpenLinksInNewActivity, true);
                intent.PutExtra(WebviewActivity.ExtraShowVersionHeader, true);
                intent.PutExtra(WebviewActivity.ExtraTitle, Resources.GetString(Resource.String.Privacy));
                StartActivity(intent);
            };
        }
    }
}