using Android.Renderscripts;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Currency.Model;

namespace MyCC.Ui.Android.Data.Get
{
	public class ReferenceValueItem
	{
		public string FormattedAmount => $"{Amount * Value:#,0.########}";

		public string FormattedRate => $"{Value:#,0.########}";

		public decimal Value;
		public decimal Amount;
		public string CurrencyCode;

		public ReferenceValueItem(decimal amount, ExchangeRate rate)
		{
			Amount = amount;
			Value = rate?.Rate ?? 0;
			CurrencyCode = rate?.SecondaryCurrencyCode;
		}

	}
}