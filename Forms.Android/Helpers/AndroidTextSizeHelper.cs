using System;
using Android.Graphics;
using Android.Widget;
using MyCC.Forms.Android.Helpers;
using MyCC.Forms.helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidTextSizeHelper))]
namespace MyCC.Forms.Android.Helpers
{
    public class AndroidTextSizeHelper : ITextSizeHelper
    {
        public Tuple<double, double> CalculateWidth(string text, float? fontsize = null, bool bold = false)
        {
            var bounds = new Rect();
            var textView = new TextView(Xamarin.Forms.Forms.Context);
            if (fontsize.HasValue)
            {
                textView.TextSize = fontsize.Value;
            }
            textView.Typeface = bold ? Typeface.DefaultBold : Typeface.Default;
            textView.Paint.GetTextBounds(text, 0, text.Length, bounds);
            var length = bounds.Width() / global::Android.Content.Res.Resources.System.DisplayMetrics.ScaledDensity;
            var height = bounds.Width() / global::Android.Content.Res.Resources.System.DisplayMetrics.ScaledDensity;
            return Tuple.Create((double)height, (double)length);
        }

    }
}
