using System.ComponentModel;
using MyCC.Forms.Constants;
using MyCC.Forms.iOS.renderer;
using MyCC.Forms.View.Components.BaseComponents;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ThemeColorProgressBar), typeof(ThemeColorProgressBarRenderer))]
namespace MyCC.Forms.iOS.renderer
{
    public class ThemeColorProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);
            UpdateBarColor();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            UpdateBarColor();
        }

        private void UpdateBarColor()
        {
            Control.TintColor = AppConstants.ThemeColor.ToUIColor();
        }
    }
}