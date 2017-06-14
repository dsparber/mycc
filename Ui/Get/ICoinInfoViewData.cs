using System;
using System.Collections.Generic;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Get
{
    public interface ICoinInfoViewData
    {
        IEnumerable<(string Name, string WebLink)> Explorer(string currencyId);

        CoinInfoItem GetInfos(string currencyId);
        bool InfosAvailable(string currencyId);

        HeaderItem GetHeaderData(string currencyId);
        DateTime LastUpdate(string currencyId);

        IEnumerable<ReferenceValueItem> ReferenceValues(string currencyId);
        List<SortButtonItem> SortButtons { get; }
    }
}