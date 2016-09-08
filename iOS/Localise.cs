using System;
using System.Globalization;
using Foundation;
using System.Threading;
using resources;
using Xamarin.Forms;

[assembly: Dependency(typeof(MyCryptos.iOS.Localise))]

namespace MyCryptos.iOS
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
					if (pref == "pt")
						pref = "pt-BR"; // get the correct Brazilian language strings from the PCL RESX (note the local iOS folder is still "pt")
					else
						pref = "pt-PT"; // Portugal
				}
				netLanguage = pref.Replace("_", "-");
			}

			// this gets called a lot - try/catch can be expensive so consider caching or something
			CultureInfo ci = null;
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