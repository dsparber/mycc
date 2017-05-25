using System.ComponentModel;
using MyCC.Forms.iOS.renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ProgressBar), typeof(CustomProgressBarRenderer))]
namespace MyCC.Forms.iOS.renderer
{
    public class CustomProgressBarRenderer : ProgressBarRenderer
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
            Control.TintColor = Color.White.ToUIColor();
            Control.TrackTintColor = Color.Transparent.ToUIColor();
        }
    }
}