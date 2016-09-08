using System.Threading.Tasks;
using data.repositories.account;
using data.storage;
using models;

namespace tasks
{
	public class AppTasks
	{
		Task fastFetchTaskInstance;
		Task fetchTaskInstance;
		Task addAccountTaskInstance;
		Task deleteAccountTaskInstance;

		public Task FastFetchTask { get { return fastFetchTaskInstance; } }
		public Task FetchTask { get { return fetchTaskInstance; } }
		public Task AddAccountTask { get { return addAccountTaskInstance; } }
		public Task DeleteAccountTask { get { return addAccountTaskInstance; } }

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

		public bool IsFastFetchTaskFinished
		{
			get
			{
				return fastFetchTaskInstance != null && fastFetchTaskInstance.IsCompleted;
			}
		}

		public bool IsFetchTaskFinished
		{
			get
			{
				return fetchTaskInstance != null && fetchTaskInstance.IsCompleted;
			}
		}

		public bool IsAddAccountTaskFinished
		{
			get
			{
				return addAccountTaskInstance != null && addAccountTaskInstance.IsCompleted;
			}
		}

		public bool IsDeleteAccountTaskFinished
		{
			get
			{
				return deleteAccountTaskInstance != null && deleteAccountTaskInstance.IsCompleted;
			}
		}

		public bool IsFastFetchTaskStarted
		{
			get
			{
				return fastFetchTaskInstance != null && (fastFetchTaskInstance.Status.Equals(TaskStatus.Running) || fastFetchTaskInstance.IsCompleted);
			}
		}

		public bool IsFetchTaskStarted
		{
			get
			{
				return fetchTaskInstance != null && (fetchTaskInstance.Status.Equals(TaskStatus.Running) || fetchTaskInstance.IsCompleted);
			}
		}

		public bool IsAddAccountTaskStarted
		{
			get
			{
				return addAccountTaskInstance != null && (addAccountTaskInstance.Status.Equals(TaskStatus.Running) || addAccountTaskInstance.IsCompleted);
			}
		}

		public bool IsDeleteAccountTaskStarted
		{
			get
			{
				return deleteAccountTaskInstance != null && (deleteAccountTaskInstance.Status.Equals(TaskStatus.Running) || deleteAccountTaskInstance.IsCompleted);
			}
		}

		Task fastFetchTask()
		{
			var accountFastFetchTask = AccountStorage.Instance.FetchFast();
			var currencyFastFetchTask = CurrencyStorage.Instance.FetchFast();
			var exchangeRateFastFetchTask = ExchangeRateStorage.Instance.FetchFast();

			return Task.WhenAll(accountFastFetchTask, currencyFastFetchTask, exchangeRateFastFetchTask);
		}

		Task fetchTask()
		{
			var accountFetchTask = AccountStorage.Instance.Fetch();
			var currencyFetchTask = CurrencyStorage.Instance.Fetch();
			var exchangeRateFetchTask = ExchangeRateStorage.Instance.Fetch();

			return Task.WhenAll(accountFetchTask, currencyFetchTask, exchangeRateFetchTask);
		}

		async Task addAccountTask(Account account)
		{
			await (await AccountStorage.Instance.Repositories()).Find(r => r is LocalAccountRepository).Add(account);
		}

		async Task deleteAccountTask(Account account)
		{
			await (await AccountStorage.Instance.Repositories()).Find(r => r is LocalAccountRepository).Delete(account);
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
}

