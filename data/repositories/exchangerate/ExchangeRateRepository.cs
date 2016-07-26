using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public abstract class ExchangeRateRepository
	{
		public List<ExchangeRate> ExchangeRates;
		public string RepositoryName;

		public ExchangeRateRepository()
		{
			ExchangeRates = new List<ExchangeRate>();
		}

		public abstract Task FetchAvailableRates();

		public abstract Task FetchAvailableRatesFast();

		public abstract Task FetchExchangeRate(ExchangeRate exchangeRate);

		public abstract Task FetchExchangeRateFast(ExchangeRate exchangeRate);

		protected async Task FetchFromDatabase()
		{
			// TODO Read Database
			throw new NotImplementedException();
		}

		protected async Task WriteToDatabase()
		{
			// TODO Write to Database
			throw new NotImplementedException();
		}
	}
}

