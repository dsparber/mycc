using System.Globalization;

namespace MyCC.Forms.Resources
{
    public interface ILocalise
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();
    }
}

