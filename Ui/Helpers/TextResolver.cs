using Xamarin.Forms;

namespace MyCC.Ui.Helpers
{
    public static class TextResolver
    {
        internal static ITextResolver Instance => DependencyService.Get<ITextResolver>();
    }
}

