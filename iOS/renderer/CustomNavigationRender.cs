using constants;
using MyCryptos.iOS.renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRender))]
namespace MyCryptos.iOS.renderer
{
    internal class CustomNavigationRender : NavigationRenderer
    {
        private UIImage backgroudImage;
        private UIImage shadowImage;
        private UIImage emptyImage;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            backgroudImage = NavigationBar.GetBackgroundImage(UIBarMetrics.Default);
            shadowImage = NavigationBar.ShadowImage;
            emptyImage = new UIImage();

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
                NavigationBar.SetBackgroundImage(emptyImage, UIBarMetrics.Default);
                NavigationBar.ShadowImage = emptyImage;
                NavigationBar.TintColor = Color.White.ToUIColor();
                NavigationBar.BarTintColor = AppConstants.ThemeColor.ToUIColor();
                NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
            }
            else
            {
                NavigationBar.SetBackgroundImage(backgroudImage, UIBarMetrics.Default);
                NavigationBar.ShadowImage = shadowImage;
                NavigationBar.TintColor = AppConstants.ThemeColor.ToUIColor();
                NavigationBar.BarTintColor = UIColor.White;
                NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.Black
                };
            }
        }
    }
}
