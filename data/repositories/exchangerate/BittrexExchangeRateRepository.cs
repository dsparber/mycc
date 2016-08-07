﻿using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public class BittrexExchangeRateRepository : OnlineExchangeRateRepository
	{
		public BittrexExchangeRateRepository(int repositoryId) : base(repositoryId) { }

		public override async Task Fetch()
		{
			// TODO Implement

			await WriteToDatabase();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			// TODO Implement

			await WriteToDatabase();
		}
	}
}

