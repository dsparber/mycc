﻿using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Rates;

namespace MyCC.Core.Tasks
{
    public static partial class ApplicationTasks
    {
        public static async Task LoadEverything(Action whenFinished = null)
        {
            await CurrencyStorage.Instance.LoadFromDatabase();
            await AccountStorage.Instance.LoadFromDatabase();
            await ExchangeRatesStorage.Instance.LoadRates();
            whenFinished?.Invoke();
        }
    }
}