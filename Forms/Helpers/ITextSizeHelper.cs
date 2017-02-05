using System;

namespace MyCC.Forms.Helpers
{
    public interface ITextSizeHelper
    {
        // https://forums.xamarin.com/discussion/67545/how-to-calculate-or-measure-width-of-a-string
        Tuple<double, double> CalculateWidth(string text, float? fontsize = null, bool bold = false);
    }
}
