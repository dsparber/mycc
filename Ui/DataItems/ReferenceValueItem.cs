using MyCC.Core.Currencies;
using MyCC.Ui.Helpers;

namespace MyCC.Ui.DataItems
{
    public class ReferenceValueItem
    {
        public string FormattedAmount => (_amount * Rate).To8DigitString();

        public string FormattedRate => Rate.To8DigitString();

        public readonly decimal Rate;
        public readonly string CurrencyCode;

        private readonly decimal _amount;

        public ReferenceValueItem(decimal amount, decimal? rate, string currencyId)
        {
            _amount = amount;
            Rate = rate ?? 0;
            CurrencyCode = currencyId.Code();
        }

    }
}