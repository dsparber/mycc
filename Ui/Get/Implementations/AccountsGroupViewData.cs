using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Get.Implementations
{
    internal class AccountsGroupViewData : IAccountsGroupViewData
    {
        public HeaderItem HeaderData(string currencyId)
        {
            var money = new Money(EnabledAccountsItems(currencyId).Sum(a => a.Amount), currencyId.Find());

            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { currencyId })
                .Select(x => new Money(money.Amount * MyccUtil.Rates.GetRate(new RateDescriptor(currencyId, x))?.Rate ?? 0, x.Find())).
                OrderBy(m => m.Currency.Code);

            return new HeaderItem(money.MaxTwoDigits(),
                additionalReferences.Any() ? string.Join(" / ", additionalReferences.Select(m => m.MaxTwoDigits())) : currencyId.FindName());
        }


        public IEnumerable<ReferenceValueItem> ReferenceItems(string currencyId)
        {
            var money = new Money(EnabledAccountsItems(currencyId).Sum(a => a.Amount), currencyId.Find());

            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { money.Currency.Id })
                .Select(c =>
                {
                    var rate = MyccUtil.Rates.GetRate(new RateDescriptor(money.Currency.Id, c));
                    return new ReferenceValueItem(money.Amount, rate?.Rate, c);
                })
                .OrderByWithDirection(c => SortOrderReference == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirectionReference == SortDirection.Ascending);
        }

        public DateTime LastUpdate(string currencyId)
        {
            var online = AccountStorage.AccountsWithCurrency(currencyId).Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.AccountsWithCurrency(currencyId).Select(a => a.LastUpdate).DefaultIfEmpty(DateTime.Now).Max();
            var ratesTime = MyccUtil.Rates.LastUpdateFor(currencyId);

            return online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;
        }

        public IEnumerable<AccountItem> EnabledAccountsItems(string currencyId)
        {
            return AccountStorage.AccountsWithCurrency(currencyId).Where(a => a.IsEnabled).Select(account => new AccountItem(account))
                                 .OrderByWithDirection(a => SortOrderAccounts == SortOrder.Alphabetical ? a.Name as object : a.Amount, SortDirectionAccounts == SortDirection.Ascending);
        }

        public bool HasAccounts(string currencyId) => EnabledAccountsItems(currencyId).Any() || DisabledAccountsItems(currencyId).Any();


        public string ReferenceTableHeader(string currencyId)
        {
            var money = new Money(EnabledAccountsItems(currencyId).Sum(a => a.Amount), currencyId.Find());

            return string.Format(money.Amount == 1 ? TextResolver.Instance.IsEqualTo : TextResolver.Instance.AreEqualTo, money);
        }

        public IEnumerable<AccountItem> DisabledAccountsItems(string currencyId)
        {
            return AccountStorage.AccountsWithCurrency(currencyId).Where(a => !a.IsEnabled).Select(account => new AccountItem(account))
                                 .OrderByWithDirection(a => SortOrderAccounts == SortOrder.Alphabetical ? a.Name as object : a.Amount, SortDirectionAccounts == SortDirection.Ascending);
        }

        public List<SortButtonItem> SortButtonsReference => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = TextResolver.Instance.Amount,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrderReference, SortDirectionReference, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSortReference(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = TextResolver.Instance.Currency,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrderReference, SortDirectionReference, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSortReference(SortOrder.Alphabetical)
            }
        };

        public List<SortButtonItem> SortButtonsAccounts => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = TextResolver.Instance.Name,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrderAccounts, SortDirectionAccounts, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSortAccounts(SortOrder.Alphabetical)
            },
            new SortButtonItem
            {
                Text = TextResolver.Instance.Amount,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrderAccounts, SortDirectionAccounts, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSortAccounts(SortOrder.ByValue)
            },
        };

        private void OnSortAccounts(SortOrder sortOrder)
        {
            SortDirectionAccounts = SortDirectionHelper.GetNewSortDirection(SortOrderAccounts, SortDirectionAccounts, sortOrder);
            SortOrderAccounts = sortOrder;
            Messaging.Sort.Accounts.Send();
        }

        private void OnSortReference(SortOrder sortOrder)
        {
            SortDirectionReference = SortDirectionHelper.GetNewSortDirection(SortOrderReference, SortDirectionReference, sortOrder);
            SortOrderReference = sortOrder;
            Messaging.Sort.ReferenceTables.Send();
        }



        private SortOrder SortOrderReference
        {
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private SortDirection SortDirectionReference
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }

        private SortOrder SortOrderAccounts
        {
            get => ApplicationSettings.SortOrderAccounts;
            set => ApplicationSettings.SortOrderAccounts = value;
        }

        private SortDirection SortDirectionAccounts
        {
            get => ApplicationSettings.SortDirectionAccounts;
            set => ApplicationSettings.SortDirectionAccounts = value;
        }
    }
}