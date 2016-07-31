using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using models;

namespace MyCryptos
{
	public interface CurrencyAPI
	{
		Task<ExchangeRate> GetExchangeRateAsync (ExchangeRate exchangeRate);

		Task<List<ExchangeRate>> GetAvailableRatesAsync ();
	}
}

