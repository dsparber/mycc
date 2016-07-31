using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using models;

namespace MyCryptos
{
	public class BtceAPI : CurrencyAPI
	{
		private readonly static string URL = "https://btc-e.com/api/3/ticker/{0}";
		private readonly static string KEY = "last";

		HttpClient client;

		public BtceAPI ()
		{
			client = new HttpClient ();
			client.MaxResponseContentBufferSize = 256000;
		}

		public async Task<List<ExchangeRate>> GetAvailableRatesAsync ()
		{
			List<ExchangeRate> exchangeRates = new List<ExchangeRate> ();

			return exchangeRates;
		}


		public async Task<ExchangeRate> GetExchangeRateAsync (ExchangeRate exchangeRate)
		{
			var uri = new Uri (string.Format (URL, RateToUrl (exchangeRate)));

			try {
				var response = await client.GetAsync (uri);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					var json = JObject.Parse (content);
					JToken rateJson = json [RateToUrl (exchangeRate)];
					var rate = (decimal)rateJson [KEY];
					exchangeRate.Rate = rate;
				}
			} catch (Exception e) {
				Debug.WriteLine (@"ERROR {0}", e.Message);
			}
			return exchangeRate;
		}

		private String RateToUrl (ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Abbreviation.ToLower () + "_" + exchangeRate.SecondaryCurrency.Abbreviation.ToLower ();
		}
	}
}

