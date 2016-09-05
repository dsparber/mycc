using System;
using System.Collections.Generic;
using System.Linq;
using constants;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace MyCryptos.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			bool result = base.FinishedLaunching(app, options);

			UINavigationBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();
			UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

			return result;
		}
	}
}

