using System.Threading.Tasks;

namespace MyCryptos
{
	public static class CurrencyConverter
	{
		public static async Task<Money> convert (Money money, Currency currency)
		{
			ExchangeRate exchangeRate = await ExchangeRateCollection.Instance.GetRate (money.Currency, currency);

			Money newMoney = new Money{ Amount = (money.Amount * exchangeRate.Rate), Currency = currency };

			return newMoney;
		}
	}
}

