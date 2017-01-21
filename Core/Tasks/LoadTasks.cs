using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Storage;
using MyCC.Core.ExchangeRate.Storage;

namespace MyCC.Core.Tasks
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