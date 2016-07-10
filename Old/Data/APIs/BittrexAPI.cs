﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace MyCryptos
{
	public class BittrexAPI : CurrencyAPI
	{
		private readonly static string URL_CURRENCY_LIST = "https://bittrex.com/api/v1.1/public/getcurrencies";
		private readonly static string CURRENCY_LIST_RESULT = "result";
		private readonly static string CURRENCY_LIST_RESULT_NAME = "CurrencyLong";
		private readonly static string CURRENCY_LIST_RESULT_CURRENCY = "Currency";

		private readonly static string URL_RATE = "https://bittrex.com/api/v1.1/public/getticker?market={0}";
		private readonly static string RESULT_KEY = "result";
		private readonly static string RATE_KEY = "Last";

		HttpClient client;

		public BittrexAPI()
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;
		}



		public async Task<List<ExchangeRate>> GetAvailableRatesAsync()
		{
			List<ExchangeRate> exchangeRates = new List<ExchangeRate>();

			var uri = new Uri(URL_CURRENCY_LIST);

			try
			{
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var json = JObject.Parse(content);
					JArray result = (JArray)json[CURRENCY_LIST_RESULT];

					foreach (JToken token in result)
					{
						var name = (String)token[CURRENCY_LIST_RESULT_NAME];
						var abbr = (String)token[CURRENCY_LIST_RESULT_CURRENCY];
						Currency currency = new Currency(name, abbr);

						exchangeRates.Add(new ExchangeRate(Currency.BTC, currency));
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"ERROR {0}", e.Message);
			}

			return exchangeRates;
		}

		public async Task<ExchangeRate> GetExchangeRateAsync(ExchangeRate exchangeRate)
		{
			var uri = new Uri(string.Format(URL_RATE, RateToUrl(exchangeRate)));

			try
			{
				var response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var json = JObject.Parse(content);
					JToken rateJson = json[RESULT_KEY];
					var rate = 1 / (decimal)rateJson[RATE_KEY];
					exchangeRate.Rate = rate;
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"ERROR {0}", e.Message);
			}
			return exchangeRate;
		}

		private String RateToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Abbreviation.ToUpper() + "-" + exchangeRate.SecondaryCurrency.Abbreviation.ToUpper();
		}
	}
}
