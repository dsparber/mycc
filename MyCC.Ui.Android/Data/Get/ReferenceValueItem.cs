using MyCC.Core.Rates;

namespace MyCC.Ui.Android.Data.Get
{
	public class ReferenceValueItem
	{
		public string FormattedAmount => $"{Amount * Value:#,0.00000000}";

		public string FormattedRate => $"{Value:#,0.00000000}";

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