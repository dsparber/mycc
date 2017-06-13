using System;
using System.Collections.Generic;
using MyCC.Ui.DataItems;
using MyCC.Ui.Get.Implementations;

namespace MyCC.Ui.Get
{
    public interface IRatesOverviewData
    {
        bool IsDataAvailable { get; }

        List<RateItem> RateItemsFor(string currencyId);
        CoinHeaderItem HeaderFor(string currencyId);
        List<SortButtonItem> SortButtonsFor(string currencyId);
        DateTime LastUpdate { get; }
        IEnumerable<string> EnabledCurrencyIds { get; }
    }
}