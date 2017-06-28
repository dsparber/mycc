using MyCC.Core.Currencies;
using MyCC.Ui.Helpers;

namespace MyCC.Ui.DataItems
{
    public class ReferenceValueItem
    {
        public string FormattedAmount => (Amount * Rate).To8DigitString();

        public string FormattedRate => Rate.To8DigitString();

        public readonly decimal Rate;
        public readonly string CurrencyCode;

        public readonly decimal Amount;

        public ReferenceValueItem(decimal amount, decimal? rate, string currencyId)
        {
            Amount = amount;
            Rate = rate ?? 0;
            CurrencyCode = currencyId.Code();
        }

    }
}