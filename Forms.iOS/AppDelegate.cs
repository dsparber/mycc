using CarouselView.FormsPlugin.iOS;
using Foundation;
using MyCC.Forms.Constants;
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
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            LoadApplication(new App());
            HybridWebViewRenderer.CopyBundleDirectory("Html");

            var result = base.FinishedLaunching(uiApplication, launchOptions);

            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
            UINavigationBar.Appearance.BarTintColor = AppConstants.ThemeColor.ToUIColor();
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White
            });

            UIProgressView.Appearance.ProgressTintColor = Color.White.ToUIColor();
            UIProgressView.Appearance.TrackTintColor = Color.Transparent.ToUIColor();

            UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

            return result;
        }
    }
}

