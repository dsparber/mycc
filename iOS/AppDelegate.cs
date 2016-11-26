using constants;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;
using CarouselView.FormsPlugin.iOS;

namespace MyCryptos.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			Forms.Init();
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

			UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

			return result;
		}
	}
}

