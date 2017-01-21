using System.Globalization;

namespace MyCC.Core.Resources
{
    public interface ILocalise
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();
    }
}

