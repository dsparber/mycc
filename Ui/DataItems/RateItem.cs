using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;

namespace MyCC.Ui.DataItems
{
    public class RateItem
    {
        public string CurrencyCode => Currency.Code;
        public string FormattedValue => ReferenceValue.ToString8Digits(false);

        public Currency Currency { get; private set; }

        public Money ReferenceValue { get; private set; }

        public RateItem(Currency currency, Money referenceValue)
        {
            Currency = currency;
            ReferenceValue = referenceValue;
        }

    }
}