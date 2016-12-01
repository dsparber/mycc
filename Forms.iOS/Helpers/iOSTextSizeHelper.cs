using System;
using CoreGraphics;
using MyCryptos.Forms.helpers;
using MyCryptos.iOS.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSTextSizeHelper))]
namespace MyCryptos.iOS.Helpers
{
    public class iOSTextSizeHelper : TextSizeHelper
    {

        UILabel uiLabel;
        CGSize length;

        public Tuple<double, double> calculateWidth(string text, float? fontSize = null, bool bold = false)
        {
            uiLabel = new UILabel();
            uiLabel.Text = text;
            if (fontSize.HasValue)
            {
                uiLabel.Font = UIFont.SystemFontOfSize(fontSize.Value);
            }
            if (bold)
            {
                uiLabel.Font = UIFont.BoldSystemFontOfSize(uiLabel.Font.PointSize);
            }
            length = uiLabel.Text.StringSize(uiLabel.Font);
            return Tuple.Create((double)length.Height, (double)length.Width);
        }

    }
}
