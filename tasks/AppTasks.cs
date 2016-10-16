using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.settings;
using data.storage;
using enums;
using message;
using models;
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

		public void StartFetchTask()
		{
			if (fetchTaskInstance == null || fetchTaskInstance.IsCompleted)
			{
				fetchTaskInstance = fetchTask();
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

		public bool IsFastFetchTaskFinished { get { return isFinished(fastFetchTaskInstance); } }
		public bool IsFetchTaskFinished{ get { return isFinished(fetchTaskInstance); } }
		public bool IsAddAccountTaskFinished { get { return isFinished(addAccountTaskInstance); } }
		public bool IsDeleteAccountTaskFinished { get { return isFinished(deleteAccountTaskInstance); } }
		public bool IsMissingRatesTaskFinished { get { return isFinished(missingRatesTaskInstance); } }

		public bool IsFastFetchTaskStarted { get { return isStarted(fastFetchTaskInstance); } }
		public bool IsFetchTaskStarted { get { return isStarted(fetchTaskInstance); } }
		public bool IsAddAccountTaskStarted { get { return isStarted(addAccountTaskInstance); } }
		public bool IsDeleteAccountTaskStarted { get { return isStarted(deleteAccountTaskInstance); } }
		public bool IsMissingRatesTaskStarted { get { return isStarted(missingRatesTaskInstance); } }


		async Task fastFetchTask()
		{
			await CurrencyStorage.Instance.FetchFast();
			await AccountStorage.Instance.FetchFast();
			await AvailableRatesStorage.Instance.FetchFast();
			await ExchangeRateStorage.Instance.FetchFast();
		}

		async Task fetchTask()
		{
			await CurrencyStorage.Instance.Fetch();
			await AccountStorage.Instance.Fetch();
			await ExchangeRateStorage.Instance.Fetch();
			await AvailableRatesStorage.Instance.Fetch();
		}

		async Task addAccountTask(Account account)
		{
			await AccountStorage.Instance.AddToLocalRepository(account);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

			await ExchangeRateStorage.Instance.GetRate(account.Money.Currency, ApplicationSettings.BaseCurrency, FetchSpeedEnum.FAST);
			await ExchangeRateStorage.Instance.FetchNew();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
		}

		async Task deleteAccountTask(Account account)
		{
			await AccountStorage.Instance.RemoveFromLocalRepository(account);
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
		}

		async Task missingRatesTask(IEnumerable<ExchangeRate> rates)
		{
			MessagingCenter.Send(string.Empty, MessageConstants.StartedFetching);
			await Task.WhenAll(rates.Select(async r =>
			{
				await ExchangeRateStorage.Instance.GetRate(r.ReferenceCurrency, r.SecondaryCurrency, FetchSpeedEnum.FAST);
			}));
			await ExchangeRateStorage.Instance.FetchNew();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedExchangeRates);
			MessagingCenter.Send(string.Empty, MessageConstants.DoneFetching);
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

		static bool isStarted(Task task)
		{
			return task != null && (task.Status.Equals(TaskStatus.Running) || task.IsCompleted);
		}
		static bool isFinished(Task task)
		{
			return task != null && task.IsCompleted;
		}
	}
}

