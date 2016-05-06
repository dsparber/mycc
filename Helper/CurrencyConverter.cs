using System;

namespace MyCryptos
{
	public static class CurrencyConverter
	{
		public static Money convert (Money money, Currency currency)
		{
			ExchangeRate exchangeRate = ExchangeRateCollection.Instance.GetRate (money.Currency, currency);

			Money newMoney = new Money{ Amount = (money.Amount * exchangeRate.Rate), Currency = currency };

			return newMoney;
		}
	}
}

