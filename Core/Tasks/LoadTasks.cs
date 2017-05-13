using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates;

namespace MyCC.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        public static async Task LoadEverything(Action whenFinished = null)
        {
            await CurrencyRepositoryMapStorage.Instance.LoadFromDatabase();
            await CurrencyStorage.Instance.LoadFromDatabase();
            await AccountStorage.Instance.LoadFromDatabase();
            await ExchangeRatesStorage.Instance.LoadRates();
            whenFinished?.Invoke();
        }
    }
}