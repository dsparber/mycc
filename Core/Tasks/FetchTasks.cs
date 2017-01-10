using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Storage;
using MyCryptos.Core.Types;

namespace MyCryptos.Core.tasks
{
	public static partial class ApplicationTasks
	{
		private static Task fetchAllExchangeRatesTask;
		private static Task fetchMissingRatesTask;
		private static Task fetchCurrenciesAndAvailableRatesTask;

		public static Task FetchBalancesAndRates(Action onStarted, Action onFinished, Action<Exception> onError)
		=> fetchAllExchangeRatesTask = fetchAllExchangeRatesTask.GetTask(async () =>
		{
			try
			{
				onStarted();
				await ExchangeRateStorage.Instance.FetchOnline();
				await AccountStorage.Instance.FetchOnline();
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

		public static Task FetchCurrenciesAndAvailableRates(Action onStarted, Action onFinished, Action<Exception> onError)
		=> fetchCurrenciesAndAvailableRatesTask = fetchCurrenciesAndAvailableRatesTask.GetTask(async () =>
		{
			try
			{
				onStarted();
				await CurrencyRepositoryMapStorage.Instance.FetchOnline();
				await CurrencyStorage.Instance.FetchOnline();
				await AvailableRatesStorage.Instance.FetchOnline();
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

		public static Task FetchMissingRates(IEnumerable<ExchangeRate.Model.ExchangeRate> rates, Action onStarted, Action onFinished, Action<Exception> onError)
		=> fetchMissingRatesTask = fetchMissingRatesTask.GetTask(async () =>
		{
			var ratesList = rates.ToList();
			ratesList.RemoveAll(e => e == null);
			if (ratesList.Count == 0) return;
			try
			{
				onStarted();
				await Task.WhenAll(ratesList.Select(r => ExchangeRateHelper.GetRate(r, FetchSpeedEnum.Fast)));
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

		public static async Task FetchBalanceAndRates(OnlineFunctionalAccount account, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await account.FetchBalanceOnline();
				// TODO Fetch rates for specific currency
				await ExchangeRateStorage.Instance.FetchOnline();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		}

		public static async Task FetchBalanceAndRates(Currency.Model.Currency currency, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await Task.WhenAll(AccountStorage.Instance.AllElements.Where(a => a.Money.Currency.Equals(currency)).OfType<OnlineFunctionalAccount>().Select(a => a.FetchBalanceOnline()));
				// TODO Fetch rates for specific currency
				await ExchangeRateStorage.Instance.FetchOnline();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		}

		public static async Task FetchBalances(OnlineAccountRepository repository, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await repository.FetchOnline();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		}

		public static async Task FetchAccounts(Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await AccountStorage.Instance.FetchOnline();
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		}

		public static async Task FetchRates(List<ExchangeRate.Model.ExchangeRate> neededRates, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await ExchangeRateStorage.Instance.FetchOnline(neededRates);
			}
			catch (Exception e)
			{
				onError(e);
			}
			finally
			{
				onFinished();
			}
		}
	}
}