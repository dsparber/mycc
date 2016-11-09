﻿using constants;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

namespace MyCryptos.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			Forms.Init();
			LoadApplication(new App());
			HybridWebViewRenderer.CopyBundleDirectory("Html");

			bool result = base.FinishedLaunching(uiApplication, launchOptions);

			UINavigationBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();
			UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

			return result;
		}
	}
}

