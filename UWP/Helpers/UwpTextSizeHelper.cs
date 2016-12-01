using System;
using MyCryptos.helpers;
using MyCryptos.UWP.Helpers;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;

[assembly: Dependency(typeof(UwpTextSizeHelper))]
namespace MyCryptos.UWP.Helpers
{
    public class UwpTextSizeHelper : TextSizeHelper
    {
        public Tuple<double, double> calculateWidth(string text, float? fontsize = null, bool bold = false)
        {
            var tb = new TextBlock { Text = text };
            if (fontsize.HasValue)
            {
                tb.FontSize = fontsize.Value;
            }
            tb.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            return Tuple.Create(tb.DesiredSize.Height, tb.DesiredSize.Width);
        }

    }
}
