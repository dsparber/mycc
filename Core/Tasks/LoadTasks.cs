using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Storage;

namespace MyCryptos.Core.tasks
{
    public static partial class ApplicationTasks
    {
        private static Task loadAllEverythingTask;

        public static Task LoadEverything(Action whenFinished) => loadAllEverythingTask = loadAllEverythingTask.GetTask(async () =>
        {
            await CurrencyRepositoryMapStorage.Instance.LoadFromDatabase();
            await CurrencyStorage.Instance.LoadFromDatabase();
            await AccountStorage.Instance.LoadFromDatabase();
            await ExchangeRateStorage.Instance.LoadFromDatabase();
            await AvailableRatesStorage.Instance.LoadFromDatabase();
            whenFinished();
        });
    }
}