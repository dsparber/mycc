using System;
using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using MyCryptos.Droid.Helpers;
using MyCryptos.Forms.helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidTextSizeHelper))]
namespace MyCryptos.Droid.Helpers
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
            var length = bounds.Width() / Resources.System.DisplayMetrics.ScaledDensity;
            var height = bounds.Width() / Resources.System.DisplayMetrics.ScaledDensity;
            return Tuple.Create((double)height, (double)length);
        }

    }
}
