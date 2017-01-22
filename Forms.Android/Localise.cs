using System.Threading;
using MyCC.Core.Resources;
using MyCC.Forms.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localise))]

namespace MyCC.Forms.Android
{
    public class Localise : ILocalise
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;

            var netLanguage = androidLocale.ToString().Replace("_", "-");

            return new System.Globalization.CultureInfo(netLanguage);
        }

        public void SetLocale()
        {
            var androidLocale = Java.Util.Locale.Default; // user's preferred locale
            var netLocale = androidLocale.ToString().Replace("_", "-");
            var ci = new System.Globalization.CultureInfo(netLocale);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}