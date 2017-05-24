using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCC.Forms.Resources
{

    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private readonly CultureInfo _ci;
        private const string ResourceId = "MyCC.Forms.Resources.I18N";

        public TranslateExtension()
        {
            _ci = DependencyService.Get<ILocalise>().GetCurrentCultureInfo();
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return "";
            }

            var temp = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);

            var translation = temp.GetString(Text, _ci);
            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException($"Key '{Text}' was not found in resources '{ResourceId}' for culture '{_ci.Name}'.");
#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
