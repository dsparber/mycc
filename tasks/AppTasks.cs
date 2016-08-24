using System.Threading.Tasks;
using data.storage;

namespace tasks
{
	public class AppTasks
	{
		Task fastFetchTaskInstance;
		Task fetchTaskInstance;

		public Task FastFetchTask { get { return fastFetchTaskInstance; } }
		public Task FetchTask { get { return fetchTaskInstance; } }

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

		public bool IsFastFetchTaskFinished {
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

