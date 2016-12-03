using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Models;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Helpers;
using System;

namespace MyCryptos.Core.Tasks
{
	public static partial class ApplicationTasks
	{
		private static Task fetchAllExchangeRatesTask;
		private static Task fetchMissingRatesTask;
		private static Task fetchCurrenciesAndAvailableRatesTask;

		public static Task FetchAllExchangeRates(Action whenFinished)
		=> fetchAllExchangeRatesTask = fetchAllExchangeRatesTask.GetTask(async () =>
		{
			await ExchangeRateStorage.Instance.FetchNew();
			whenFinished();
		});

		public static Task FetchCurrenciesAndAvailableRates(Action whenFinished)
		=> fetchCurrenciesAndAvailableRatesTask = fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
		{
			await CurrencyRepositoryMapStorage.Instance.Fetch();
			await CurrencyStorage.Instance.Fetch();
			await AvailableRatesStorage.Instance.Fetch();
			whenFinished();
		});

		public static Task FetchMissingRates(IEnumerable<ExchangeRate> rates, Action whenFinished)
		=> fetchMissingRatesTask = fetchMissingRatesTask.GetTask(async () =>
		{
			var ratesList = rates.ToList();
			ratesList.RemoveAll(e => e == null);
			if (ratesList.Count == 0) return;

			await Task.WhenAll(ratesList.Select(r => ExchangeRateHelper.GetRate(r, FetchSpeedEnum.FAST)));
			await ExchangeRateStorage.Instance.FetchNew();
			whenFinished();
		});
	}
}