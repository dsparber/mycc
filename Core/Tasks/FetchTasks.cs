using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
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

		public static Task FetchAllExchangeRates(Action whenFinished, Action<Exception> onError)
		=> fetchAllExchangeRatesTask = fetchAllExchangeRatesTask.GetTask(async () =>
		{
			try
			{
				await ExchangeRateStorage.Instance.Fetch();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				whenFinished();
			}
		});

		public static Task FetchCurrenciesAndAvailableRates(Action onFinished, Action<Exception> onError)
		=> fetchCurrenciesAndAvailableRatesTask = fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
		{
			try
			{
				await CurrencyRepositoryMapStorage.Instance.Fetch();
				await CurrencyStorage.Instance.Fetch();
				await AvailableRatesStorage.Instance.Fetch();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		});

		public static Task FetchMissingRates(IEnumerable<ExchangeRate> rates, Action onFinished, Action<Exception> onError)
		=> fetchMissingRatesTask = fetchMissingRatesTask.GetTask(async () =>
		{
			var ratesList = rates.ToList();
			ratesList.RemoveAll(e => e == null);
			if (ratesList.Count == 0) return;
			try
			{
				await Task.WhenAll(ratesList.Select(r => ExchangeRateHelper.GetRate(r, FetchSpeedEnum.FAST)));
				await ExchangeRateStorage.Instance.FetchNew();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		});
	}
}