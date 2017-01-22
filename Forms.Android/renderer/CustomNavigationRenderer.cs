using Android.App;
using Android.Graphics.Drawables;
using MyCC.Forms.Android.renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace MyCC.Forms.Android.renderer
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);

            RemoveAppIconFromActionBar();
        }

        private void RemoveAppIconFromActionBar()
        {
            var actionBar = ((Activity)Context).ActionBar;
            var icon = new ColorDrawable(Color.Transparent.ToAndroid());
            actionBar.SetIcon(icon);
        }
    }
}