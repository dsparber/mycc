using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.settings;
using data.storage;
using enums;
using message;
using MyCryptos.helpers;
using MyCryptos.models;
using Xamarin.Forms;

namespace tasks
{
	public class AppTasks
	{
		Task fastFetchTaskInstance;
		Task fetchTaskInstance;
		Task addAccountTaskInstance;
		Task deleteAccountTaskInstance;
		Task missingRatesTaskInstance;

		public Task FastFetchTask { get { return fastFetchTaskInstance; } }
		public Task FetchTask { get { return fetchTaskInstance; } }
		public Task AddAccountTask { get { return addAccountTaskInstance; } }
		public Task DeleteAccountTask { get { return deleteAccountTaskInstance; } }
		public Task MissingRatesTask { get { return missingRatesTaskInstance; } }


		public void StartFastFetchTask()
		{
			if (fastFetchTaskInstance == null || fastFetchTaskInstance.IsCompleted)
			{
				fastFetchTaskInstance = fastFetchTask();
			}
		}

		public void StartFetchTask(bool includeFastFetchTask)
		{
			if (fetchTaskInstance == null || fetchTaskInstance.IsCompleted)
			{
				fetchTaskInstance = fetchTask(includeFastFetchTask);
			}
		}

		public void StartAddAccountTask(Account account)
		{
			if (addAccountTaskInstance == null || addAccountTaskInstance.IsCompleted)
			{
				addAccountTaskInstance = addAccountTask(account);
			}
		}

		public void StartDeleteAccountTask(Account account)
		{
			if (deleteAccountTaskInstance == null || deleteAccountTaskInstance.IsCompleted)
			{
				deleteAccountTaskInstance = deleteAccountTask(account);
			}
		}

		public void StartMissingRatesTask(IEnumerable<ExchangeRate> rates)
		{
			if (missingRatesTaskInstance == null || missingRatesTaskInstance.IsCompleted)
			{
				missingRatesTaskInstance = missingRatesTask(rates);
			}
		}

		public bool IsFastFetchTaskFinished { get { return fastFetchTaskInstance.IsFinished(); } }
		public bool IsFetchTaskFinished { get { return fetchTaskInstance.IsFinished(); } }
		public bool IsAddAccountTaskFinished { get { return addAccountTaskInstance.IsFinished(); } }
		public bool IsDeleteAccountTaskFinished { get { return deleteAccountTaskInstance.IsFinished(); } }
		public bool IsMissingRatesTaskFinished { get { return missingRatesTaskInstance.IsFinished(); } }

		public bool IsFastFetchTaskStarted { get { return fastFetchTaskInstance.IsStarted(); } }
		public bool IsFetchTaskStarted { get { return fetchTaskInstance.IsStarted(); } }
		public bool IsAddAccountTaskStarted { get { return addAccountTaskInstance.IsStarted(); } }
		public bool IsDeleteAccountTaskStarted { get { return deleteAccountTaskInstance.IsStarted(); } }
		public bool IsMissingRatesTaskStarted { get { return missingRatesTaskInstance.IsStarted(); } }


		async Task fastFetchTask()
		{
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.StartedFetching);
			await CurrencyRepositoryMapStorage.Instance.FetchFast();
			await CurrencyStorage.Instance.FetchFast();
			await AccountStorage.Instance.FetchFast();
			await ExchangeRateStorage.Instance.FetchFast();
			await AvailableRatesStorage.Instance.FetchFast();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.DoneFetching);
		}

		async Task fetchTask(bool includeFastFetchTask)
		{
			if (includeFastFetchTask) {
				await fastFetchTask();
			}
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.SLOW), MessageConstants.StartedFetching);
			await CurrencyRepositoryMapStorage.Instance.Fetch();
			await CurrencyStorage.Instance.Fetch();
			await AccountStorage.Instance.Fetch();
			await ExchangeRateStorage.Instance.Fetch();
			await AvailableRatesStorage.Instance.Fetch();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.SLOW), MessageConstants.DoneFetching);
		}

		async Task addAccountTask(Account account)
		{
			await AccountStorage.Instance.LocalRepository.Add(account);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

			await ExchangeRateHelper.GetRate(account.Money.Currency, ApplicationSettings.BaseCurrency, FetchSpeedEnum.FAST);
			await ExchangeRateStorage.Instance.FetchNew();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
		}

		async Task deleteAccountTask(Account account)
		{
			await AccountStorage.Instance.LocalRepository.Remove(account);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
		}

		async Task missingRatesTask(IEnumerable<ExchangeRate> rates)
		{
			var ratesList = rates.ToList();
			ratesList.RemoveAll(e => e == null);
			if (ratesList.Count > 0)
			{
				MessagingCenter.Send(string.Empty, MessageConstants.StartedFetching);
				await Task.WhenAll(ratesList.Select(async r =>
				{
					await ExchangeRateHelper.GetRate(r.ReferenceCurrency, r.SecondaryCurrency, FetchSpeedEnum.FAST);
				}));
				await ExchangeRateStorage.Instance.FetchNew();
				MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
				MessagingCenter.Send(string.Empty, MessageConstants.DoneFetching);
			}
		}


		static AppTasks instance { get; set; }
		public static AppTasks Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AppTasks();
				}
				return instance;
			}
		}


	}

	public static class TaskHelper
	{
		public static bool IsStarted(this Task task)
		{
			return task != null && (task.Status.Equals(TaskStatus.Running) || task.IsCompleted);
		}
		public static bool IsFinished(this Task task)
		{
			return task != null && task.IsCompleted;
		}
	}
}

