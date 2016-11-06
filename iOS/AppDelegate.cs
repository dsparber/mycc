using constants;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

namespace MyCryptos.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();
            LoadApplication(new App());
			HybridWebViewRenderer.CopyBundleDirectory("Html");

            bool result = base.FinishedLaunching(app, options);

			UINavigationBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();
			UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

			return result;
		}
	}
}

