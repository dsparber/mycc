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
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			StartActivity(typeof(MainActivity));
		}
	}
}

