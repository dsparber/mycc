using Android.App;
using Android.Graphics.Drawables;
using constants;
using MyCryptos.resources;
using renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.App.ActivityManager;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace renderer
{
	public class CustomNavigationRenderer : NavigationRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);

			RemoveAppIconFromActionBar();
		}

		void RemoveAppIconFromActionBar()
		{
			var actionBar = ((Activity)Context).ActionBar;
			actionBar.SetIcon(new ColorDrawable(Color.Transparent.ToAndroid()));            
        }
	}
}