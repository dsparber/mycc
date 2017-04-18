using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;

namespace MyCC.Core.Tasks
{
	public static partial class ApplicationTasks
	{
		public static async Task FetchRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
		{
			try
			{
				onStarted?.Invoke();
				await ExchangeRatesStorage.Instance.UpdateRates(progressCallback);
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

		public static async Task FetchCurrenciesAndAvailableRates(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
		{
			try
			{
				onStarted?.Invoke();
				await CurrencyRepositoryMapStorage.Instance.FetchOnline();
				await CurrencyStorage.Instance.FetchOnline();
				await ExchangeRatesStorage.Instance.FetchAvailableRates();
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

		public static async Task FetchMissingRates(IEnumerable<ExchangeRate> rates, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
		{
			var ratesList = rates.ToList();
			ratesList.RemoveAll(e => e == null);
			if (ratesList.Count == 0) return;
			try
			{
				onStarted?.Invoke();
				await ExchangeRateHelper.FetchMissingRatesFor(ratesList, progressCallback);
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

		public static async Task FetchBalance(FunctionalAccount account, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				var onlineFunctionalAccount = account as OnlineFunctionalAccount;
				if (onlineFunctionalAccount != null) await onlineFunctionalAccount.FetchBalanceOnline();
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

		public static Task FetchRates(FunctionalAccount account, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
			=> FetchRates(account.Money.Currency, onStarted, onFinished, onError, progressCallback);


		public static async Task FetchRates(Currency.Model.Currency currency, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
		{
			try
			{
				onStarted?.Invoke();
				var ratesToUpdate = ApplicationSettings.AllReferenceCurrencies.Select(c => new ExchangeRate(currency, c));
				await ExchangeRateHelper.UpdateRates(ratesToUpdate, progressCallback);
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

		public static async Task FetchBalance(Currency.Model.Currency currency, Action onStarted, Action onFinished, Action<Exception> onError, Action<double> progressCallback)
		{
			try
			{
				onStarted();
				var accounts = AccountStorage.AccountsWithCurrency(currency).OfType<OnlineFunctionalAccount>().ToList();
				var progress = .0;
				await Task.WhenAll(accounts.Select(async a =>
				{
					await a.FetchBalanceOnline();
					progress += 1;
					progressCallback(progress / accounts.Count);
				}));
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

		public static async Task FetchAccounts(Action onStarted = null, Action onFinished = null, Action<Exception> onError = null, Action<double> progressCallback = null)
		{
			try
			{
				onStarted?.Invoke();
				await AccountStorage.Instance.FetchOnline(progressCallback);
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

		public static async Task FetchRates(IEnumerable<ExchangeRate> neededRates, Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await ExchangeRateHelper.UpdateRates(neededRates);
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

		public static async Task UpdateRates(Action onStarted, Action onFinished, Action<Exception> onError)
		{
			try
			{
				onStarted();
				await ExchangeRatesStorage.Instance.UpdateRates(d => { });
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

		public static async Task FetchBitcoinDollarRates(Action onStarted, Action onFinished, Action<Exception> onError, Action<double> progressCallback)
		{
			try
			{
				onStarted();
				await ExchangeRateHelper.FetchDollarBitcoinRates(progressCallback);
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

		public static async Task FetchCoinInfo(Currency.Model.Currency coin, Action onStarted = null, Action onFinished = null, Action<Exception> onError = null)
		{
			try
			{
				onStarted?.Invoke();
				await CoinInfoStorage.Instance.FetchInfo(coin);
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
			finally
			{
				onFinished?.Invoke();
			}
		}

	}
}