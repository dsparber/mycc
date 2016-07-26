using System;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public class BittrexExchangeRateRepository : OnlineExchangeRateRepository
	{
		public override async Task FetchAvailableRates()
		{
			// TODO Implement
			throw new NotImplementedException();

			await WriteToDatabase();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			// TODO Implement
			throw new NotImplementedException();

			await WriteToDatabase();
		}
	}
}

