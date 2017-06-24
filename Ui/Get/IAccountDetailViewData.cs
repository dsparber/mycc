using System;
using System.Collections.Generic;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Get
{
    public interface IAccountDetailViewData
    {
        string CurrencyId(int accountId);
        int RepositoryId(int accountId);
        bool IsLocal(int accountId);

        bool ShowQrCodePossible(int accountId);
        bool BlockExplorerCallAllowed(int accountId);
        bool ShowAccountSource(int accountId);
        bool ShowAccountAddress(int accountId);

        string AccountName(int accountId);
        string AccountType(int accountId);
        string AccountSource(int accountId);
        string AccountAddressString(int accountId);
        string AddressClickUrl(int accountId);

        string ReferenceTableHeader(int accountId);
        IEnumerable<ReferenceValueItem> GetReferenceItems(int accountId);
        List<SortButtonItem> SortButtons { get; }

        HeaderItem HeaderData(int accountId);
        DateTime LastUpdate(int accountId);

    }
}