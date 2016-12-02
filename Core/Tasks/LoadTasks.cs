using System.Threading.Tasks;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        private static Task loadAllEverythingTask;

        public static Task LoadEverything() => loadAllEverythingTask = loadAllEverythingTask.GetTask(async () =>
        {
            await CurrencyRepositoryMapStorage.Instance.FetchFast();
            await CurrencyStorage.Instance.FetchFast();
            await AccountStorage.Instance.FetchFast();
            await ExchangeRateStorage.Instance.FetchFast();
            await AvailableRatesStorage.Instance.FetchFast();
        });
    }
}