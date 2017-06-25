using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MyCC.Forms.Resources
{

    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private const string ResourceId = "MyCC.Forms.Resources.I18N";

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null) return string.Empty;

            var res = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            return res.GetString(Text, CultureInfo.CurrentCulture) ?? res.GetString(Text, CultureInfo.InvariantCulture) ?? Text;
        }
    }
}
