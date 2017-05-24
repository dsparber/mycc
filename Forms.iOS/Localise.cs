using System.Globalization;
using System.Threading;
using Foundation;
using MyCC.Forms.iOS;
using MyCC.Forms.Resources;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localise))]

namespace MyCC.Forms.iOS
{
    public class Localise : ILocalise
    {
        public void SetLocale()
        {
            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            var netLocale = iosLocaleAuto.Replace("_", "-");
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLocale);
            }
            catch
            {
                ci = GetCurrentCultureInfo();
            }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var prefLanguageOnly = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];

                // Apple treats portuguese fallbacks in a strange way
                // https://developer.apple.com/library/ios/documentation/MacOSX/Conceptual/BPInternational/LocalizingYourApp/LocalizingYourApp.html
                // "For example, use pt as the language ID for Portuguese as it is used in Brazil and pt-PT as the language ID for Portuguese as it is used in Portugal"
                prefLanguageOnly = pref.Substring(0, 2);
                if (prefLanguageOnly == "pt")
                {
                    pref = pref == "pt" ? "pt-BR" : "pt-PT";
                }
                netLanguage = pref.Replace("_", "-");
            }

            // this gets called a lot - try/catch can be expensive so consider caching or something
            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch
            {
                ci = new CultureInfo(prefLanguageOnly);
            }

            return ci;
        }
    }
}