using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using models;

namespace data.repositories.currency
{
	public abstract class CurrencyRepository
	{
		public List<Currency> Currencies;
		public string RepositoryName;

		public CurrencyRepository()
		{
			Currencies = new List<Currency>();
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

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

