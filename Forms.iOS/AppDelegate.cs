using CarouselView.FormsPlugin.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System;
using System.Globalization;
using HockeyApp.iOS;
using MyCC.Core.Database;
using MyCC.Forms.Constants;
using MyCC.Forms.iOS.data.database;
using MyCC.Forms.Resources;
using MyCC.Ui.Messages;
using Refractored.XamForms.PullToRefresh.iOS;

namespace MyCC.Forms.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            I18N.Culture = CultureInfo.CurrentCulture;

            Xamarin.Forms.Forms.Init();
            DatabaseUtil.SqLiteConnection = new SqLiteConnectionIos();
            CarouselViewRenderer.Init();
            PullToRefreshLayoutRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            var x = (int)UIScreen.MainScreen.Bounds.Width;
            var y = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenHeight = Math.Max(x, y);

            Messaging.Status.DarkStatusBar.Subscribe(this, b => UIApplication.SharedApplication.SetStatusBarStyle(b ? UIStatusBarStyle.Default : UIStatusBarStyle.LightContent, false));

            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
            UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
            UINavigationBar.Appearance.BarTintColor = AppConstants.ThemeColor.ToUIColor();
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = UIColor.White
            });

            UITabBar.Appearance.TintColor = AppConstants.ThemeColor.ToUIColor();

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("3e42251c3ae84c498abf08fbdd56a818");
            manager.DisableUpdateManager = true;
            manager.StartManager();

            LoadApplication(new App());
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}

