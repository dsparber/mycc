using Android.App;
using Android.Content.PM;
using Android.OS;
using CarouselView.FormsPlugin.Android;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using Refractored.XamForms.PullToRefresh.Droid;
using Xamarin.Forms.Platform.Android;
using Java.Lang;

namespace MyCC.Forms.Android
{
    [Activity(Label = "MyCC", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CarouselViewRenderer.Init();
            PullToRefreshLayoutRenderer.Init();
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            var y = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            var x = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            App.ScreenHeight = Math.Max(x, y);
            App.ScreenWidth = Math.Min(x, y);

            LoadApplication(new App());

            CrashManager.Register(this, "7792ee5321a64433ace4955a1693cca5");
            MetricsManager.Register(Application, "7792ee5321a64433ace4955a1693cca5");

            ActionBar.Elevation = 0;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

