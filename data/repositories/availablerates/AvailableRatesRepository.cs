﻿using System.Threading.Tasks;
using data.repositories.exchangerate;
using data.repositories.general;
using models;

namespace data.repositories.availablerates
{
	public abstract class AvailableRatesRepository : AbstractAvailabilityRepository<ExchangeRate>
	{
		protected AvailableRatesRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override Task FetchFast()
		{
			return Fetch();
		}

		public abstract Task<ExchangeRateRepository> ExchangeRateRepository();

		public abstract ExchangeRate ExchangeRateWithCurrency(Currency currency);
	}
}
