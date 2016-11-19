using constants;
using MyCryptos.iOS.renderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRender))]
namespace MyCryptos.iOS.renderer
{
    class CustomNavigationRender : NavigationRenderer
    {

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            NavigationBar.ShadowImage = new UIImage();
        }
    }
}
