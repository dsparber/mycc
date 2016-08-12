﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using models;
using Newtonsoft.Json.Linq;

namespace data.repositories.exchangerate
{
	public class BtceExchangeRateRepository : OnlineExchangeRateRepository
	{
		const string URL = "https://btc-e.com/api/3/ticker/{0}";
		const string KEY = "last";

		const int BUFFER_SIZE = 256000;

		readonly HttpClient client;

		public BtceExchangeRateRepository() : base(ExchangeRateRepositoryDBM.DB_TYPE_BTCE_REPOSITORY)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		public override async Task Fetch()
		{
			Elements.Add(new ExchangeRate(Currency.BTC, Currency.EUR));
			Elements.Add(new ExchangeRate(Currency.BTC, Currency.USD));
			await WriteToDatabase();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			var uri = new Uri(string.Format(URL, RateToUrl(exchangeRate)));
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				JToken rateJson = json[RateToUrl(exchangeRate)];
				var rate = (decimal)rateJson[KEY];
				exchangeRate.Rate = rate;
			}

			await WriteToDatabase();
		}

		string RateToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Code.ToLower() + "_" + exchangeRate.SecondaryCurrency.Code.ToLower();
		}
	}
}
