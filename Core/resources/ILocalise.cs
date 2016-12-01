using System.Globalization;

namespace MyCryptos.Core.Resources
{
    public interface ILocalise
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();
    }
}

