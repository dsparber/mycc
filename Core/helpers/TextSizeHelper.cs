using System;

namespace MyCryptos.helpers
{
	public interface TextSizeHelper
	{
		// https://forums.xamarin.com/discussion/67545/how-to-calculate-or-measure-width-of-a-string
		Tuple<double, double> calculateWidth(string text, float? fontsize = null, bool bold = false);
	}
}
