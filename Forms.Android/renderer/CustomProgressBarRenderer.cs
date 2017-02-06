using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using MyCC.Forms.Android.renderer;
using MyCC.Forms.Constants;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(ProgressBar), typeof(CustomProgressBarRenderer))]
namespace MyCC.Forms.Android.renderer
{
    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control != null)
            {
                UpdateBarColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            UpdateBarColor();
        }

        private void UpdateBarColor()
        {
            Control.IndeterminateDrawable.SetColorFilter(AppConstants.ThemeColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.ProgressDrawable.SetColorFilter(Color.White.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.SetBackgroundColor(Color.Transparent.ToAndroid());
            Control.SetBackground(new ColorDrawable(Color.Transparent.ToAndroid()));
        }
    }
}