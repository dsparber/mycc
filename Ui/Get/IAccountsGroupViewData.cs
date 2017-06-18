using System;
using System.Collections.Generic;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Get
{
    public interface IAccountsGroupViewData
    {
        HeaderItem HeaderData(string currencyId);
        DateTime LastUpdate(string currencyId);

        string ReferenceTableHeader(string currencyId);
        IEnumerable<ReferenceValueItem> ReferenceItems(string currencyId);
        List<SortButtonItem> SortButtonsReference { get; }
        List<SortButtonItem> SortButtonsAccounts { get; }

        IEnumerable<AccountItem> DisabledAccountsItems(string currencyId);
        IEnumerable<AccountItem> EnabledAccountsItems(string currencyId);

        bool HasAccounts(string currencyId);
    }
}