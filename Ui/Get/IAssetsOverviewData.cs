using System;
using System.Collections.Generic;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Get
{
    public interface IAssetsOverviewData
    {
        bool IsDataAvailable { get; }
        bool IsGraphDataAvailable { get; }

        List<AssetItem> TableItemsFor(string currencyId);
        string GrapItemsJsFor(string currencyId);
        CoinHeaderItem HeaderFor(string currencyId);
        List<SortButtonItem> SortButtonsFor(string currencyId);
        DateTime LastUpdate { get; }
    }
}