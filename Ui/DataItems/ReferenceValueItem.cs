using MyCC.Core.Currencies;

namespace MyCC.Ui.DataItems
{
    public class ReferenceValueItem
    {
        public string FormattedAmount => $"{_amount * Rate:#,0.00000000}";

        public string FormattedRate => $"{Rate:#,0.00000000}";

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