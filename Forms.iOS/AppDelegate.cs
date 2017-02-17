using CarouselView.FormsPlugin.iOS;
using Foundation;
using HockeyApp.iOS;
using MyCC.Forms.Constants;
using Refractored.XamForms.PullToRefresh.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

namespace MyCC.Forms.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            CarouselViewRenderer.Init();
            PullToRefreshLayoutRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            LoadApplication(new App());
            HybridWebViewRenderer.CopyBundleDirectory("Html");

            var result = base.FinishedLaunching(uiApplication, launchOptions);

            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
            UINavigationBar.Appearance.BarTintColor = AppConstants.ThemeColor.ToUIColor();
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = UIColor.White
            });

            UIProgressView.Appearance.ProgressTintColor = Color.White.ToUIColor();
            UIProgressView.Appearance.TrackTintColor = Color.Transparent.ToUIColor();

            UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("3e42251c3ae84c498abf08fbdd56a818");
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();

            return result;
        }
    }
}

