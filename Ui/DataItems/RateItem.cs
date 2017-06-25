using MyCC.Core.Currencies;
using MyCC.Ui.Helpers;

namespace MyCC.Ui.DataItems
{
    public class RateItem
    {
        public readonly string CurrencyCode;
        public string FormattedValue => ReferenceValue.To8DigitString();

        public readonly decimal ReferenceValue;
        public readonly string CurrencyId;

        public RateItem(string currencyId, decimal referenceValue)
        {
            CurrencyId = currencyId;
            CurrencyCode = currencyId.Code();
            ReferenceValue = referenceValue;
        }

    }
}