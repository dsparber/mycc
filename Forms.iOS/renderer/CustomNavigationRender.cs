using MyCC.Forms.Constants;
using MyCC.Forms.iOS.renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRender))]
namespace MyCC.Forms.iOS.renderer
{
	internal class CustomNavigationRender : NavigationRenderer
	{
		private UIImage _backgroudImage;
		private UIImage _shadowImage;
		private UIImage _emptyImage;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			_backgroudImage = NavigationBar.GetBackgroundImage(UIBarMetrics.Default);
			_shadowImage = NavigationBar.ShadowImage;
			_emptyImage = new UIImage();

			SetNavigationBar();
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);
			SetNavigationBar();
		}


		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			SetNavigationBar();
		}

		private void SetNavigationBar()
		{
			var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
			var isPortrait = currentOrientation == UIInterfaceOrientation.Portrait || currentOrientation == UIInterfaceOrientation.PortraitUpsideDown;

			if (isPortrait)
			{
				UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
				NavigationBar.SetBackgroundImage(_emptyImage, UIBarMetrics.Default);
				NavigationBar.ShadowImage = _emptyImage;
				NavigationBar.TintColor = Color.White.ToUIColor();
				NavigationBar.BarTintColor = AppConstants.ThemeColor.ToUIColor();
				NavigationBar.TitleTextAttributes = new UIStringAttributes
				{
					ForegroundColor = UIColor.White
				};
			}
			else
			{
				UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
				NavigationBar.SetBackgroundImage(_backgroudImage, UIBarMetrics.Default);
				NavigationBar.ShadowImage = _shadowImage;
				NavigationBar.TintColor = AppConstants.ThemeColor.ToUIColor();
				NavigationBar.BarTintColor = UIColor.White;
				NavigationBar.TitleTextAttributes = new UIStringAttributes
				{
					ForegroundColor = UIColor.Black
				};
			}
		}
	}
}
