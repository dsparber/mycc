using Android.Renderscripts;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Rates;
using MyCC.Core.Settings;

namespace MyCC.Ui.Android.Data.Get
{
    public class ReferenceValueItem
    {
        public string FormattedAmount => $"{Amount * _rate.Rate ?? 0:#,0.########}";

        public string FormattedRate => $"{_rate.Rate ?? 0:#,0.########}";

        public string CurrencyCode => _rate.SecondaryCurrencyCode;


        public readonly decimal Amount;

        private readonly ExchangeRate _rate;

        public ReferenceValueItem(decimal amount, ExchangeRate rate)
        {
            Amount = amount;
            _rate = rate;
        }

    }
}