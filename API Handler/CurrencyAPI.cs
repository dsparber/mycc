using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCryptos
{
	public interface CurrencyAPI
	{
		Task<List<ExchangeRate>> GetExchangeRatesAsync ();
	}
}

