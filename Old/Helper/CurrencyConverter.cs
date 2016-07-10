using System;
using System.Threading.Tasks;

namespace MyCryptos
{
	public static class CurrencyConverter
	{
		public static async Task<Money> convert (Money money, Currency currency)
		{
			ExchangeRate exchangeRate = await ExchangeRateCollection.Instance.GetRate (money.Currency, currency);

			if (exchangeRate.Rate == null)
			{
				throw new ArgumentNullException();
			}

			Money newMoney = new Money{ Amount = ((decimal)(money.Amount * exchangeRate.Rate)), Currency = currency };

			return newMoney;
		}
	}
}

