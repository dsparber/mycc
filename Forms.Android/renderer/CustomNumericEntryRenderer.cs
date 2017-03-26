using System.Globalization;
using Android.Text.Method;
using MyCC.Forms.Android.renderer;
using MyCC.Forms.View.Components.BaseComponents;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NumericEntry), typeof(CustomNumericEntryRenderer))]
namespace MyCC.Forms.Android.renderer
{
    public class CustomNumericEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.KeyListener = DigitsKeyListener.GetInstance($"1234567890{CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator}");
        }
    }
}