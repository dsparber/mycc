using System.Threading.Tasks;
using data.repositories.account;
using data.repositories.currency;
using data.storage;
using models;

namespace tasks
{
	public class AppTasks
	{
		Task fastFetchTaskInstance;
		Task fetchTaskInstance;
		Task addAccountTaskInstance;

		public Task FastFetchTask { get { return fastFetchTaskInstance; } }
		public Task FetchTask { get { return fetchTaskInstance; } }
		public Task AddAccountTaskInstance { get { return addAccountTaskInstance; } }

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

		public void StartAddAccountTask(string accountName, decimal value, string currencyCode)
		{
			if (addAccountTaskInstance == null || addAccountTaskInstance.IsCompleted)
			{
				addAccountTaskInstance = addAccountTask(accountName, value, currencyCode);
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

		async Task addAccountTask(string accountName, decimal value, string currencyCode)
		{
			var currency = await CurrencyStorage.Instance.GetByString(currencyCode);
			if (currency == null)
			{
				currency = new Currency(currencyCode.ToUpper());
				await (await CurrencyStorage.Instance.Repositories()).Find(r => r is LocalCurrencyRepository).Add(currency);
				currency = await CurrencyStorage.Instance.GetByString(currencyCode);
			}

			var money = new Money(value, currency);
			var account = new Account(accountName, money);
			await (await AccountStorage.Instance.Repositories()).Find(r => r is LocalAccountRepository).Add(account);
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

