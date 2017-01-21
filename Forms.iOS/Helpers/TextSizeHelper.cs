using System;
using MyCC.Forms.helpers;
using MyCC.Forms.iOS.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(TextSizeHelper))]
namespace MyCC.Forms.iOS.Helpers
{
    public class TextSizeHelper : ITextSizeHelper
    {

        public Tuple<double, double> CalculateWidth(string text, float? fontSize = null, bool bold = false)
        {
            var uiLabel = new UILabel { Text = text };
            if (fontSize.HasValue)
            {
                uiLabel.Font = UIFont.SystemFontOfSize(fontSize.Value);
            }
            if (bold)
            {
                uiLabel.Font = UIFont.BoldSystemFontOfSize(uiLabel.Font.PointSize);
            }
            var length = uiLabel.Text.StringSize(uiLabel.Font);
            return Tuple.Create((double)length.Height, (double)length.Width);
        }

    }
}
