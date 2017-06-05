using MyCC.Core.Rates;
using MyCC.Core.Rates.Utils;

namespace MyCC.Core
{
    public static class MyccUtil
    {
        public static readonly IRatesUtil Rates = new RatesUtil();
    }
}