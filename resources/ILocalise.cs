using System.Globalization;

namespace resources
{
	public interface ILocalise
	{
		CultureInfo GetCurrentCultureInfo();

		void SetLocale();
	}
}

