using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Get
{
    public class AccountsGroupViewData
    {
        public static HeaderDataItem HeaderData(Currency currency)
        {
            var money = new Money(EnabledAccountsItems(currency).Sum(a => a.Money.Amount), currency);

            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { currency.Id })
                .Select(x => new Money(money.Amount * MyccUtil.Rates.GetRate(new RateDescriptor(currency.Id, x))?.Rate ?? 0, x.Find())).
                OrderBy(m => m.Currency.Code);

            return new HeaderDataItem(money.ToStringTwoDigits(ApplicationSettings.RoundMoney),
                additionalReferences.Any() ? string.Join(" / ", additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : currency.Name);
        }


        public static IEnumerable<ReferenceValueItem> ReferenceItems(Currency currency)
        {
            var money = new Money(EnabledAccountsItems(currency).Sum(a => a.Money.Amount), currency);

            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { money.Currency.Id })
                .Select(c =>
                {
                    var rate = MyccUtil.Rates.GetRate(new RateDescriptor(money.Currency.Id, c));
                    return new ReferenceValueItem(money.Amount, rate?.Rate, c);
                })
                .OrderByWithDirection(c => SortOrderReference == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirectionReference == SortDirection.Ascending);
        }

        public static DateTime LastUpdate(Currency currency)
        {
            var online = AccountStorage.AccountsWithCurrency(currency).Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.AccountsWithCurrency(currency).Select(a => a.LastUpdate).DefaultIfEmpty(DateTime.Now).Max();
            var ratesTime = MyccUtil.Rates.LastUpdateFor(currency.Id);

            return online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;
        }

        public static IEnumerable<Account> EnabledAccountsItems(Currency currency)
        {
            return AccountStorage.AccountsWithCurrency(currency).Where(a => a.IsEnabled)
                                 .OrderByWithDirection(a => SortOrderAccounts == SortOrder.Alphabetical ? a.Name as object : a.Money.Amount,
                                                       SortDirectionAccounts == SortDirection.Ascending);
        }

        public static Money GetEnabledSum(Currency currency) => new Money(EnabledAccountsItems(currency).Sum(a => a.Money.Amount), currency);

        public static IEnumerable<Account> DisabledAccountsItems(Currency currency)
        {
            return AccountStorage.AccountsWithCurrency(currency).Where(a => !a.IsEnabled)
                                 .OrderByWithDirection(a => SortOrderAccounts == SortOrder.Alphabetical ? a.Name as object : a.Money.Amount,
                                                       SortDirectionAccounts == SortDirection.Ascending);
        }

        public List<SortButtonItem> SortButtonsReference => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Amount,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderReference, SortDirectionReference, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSortReference(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Currency,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderReference, SortDirectionReference, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSortReference(SortOrder.Alphabetical)
            }
        };

        public List<SortButtonItem> SortButtonsAccounts => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Name,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderAccounts, SortDirectionAccounts, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSortAccounts(SortOrder.Alphabetical)
            },
            new SortButtonItem
            {
                Text = StringHelper.TextResolver.Amount,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderAccounts, SortDirectionAccounts, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSortAccounts(SortOrder.ByValue)
            },
        };

        private static void OnSortAccounts(SortOrder sortOrder)
        {
            SortDirectionAccounts = SortDirectionHelper.GetNewSortDirection(SortOrderAccounts, SortDirectionAccounts, sortOrder);
            SortOrderAccounts = sortOrder;
            Messaging.UiUpdate.ReferenceTables.Send();
        }

        private static void OnSortReference(SortOrder sortOrder)
        {
            SortDirectionReference = SortDirectionHelper.GetNewSortDirection(SortOrderReference, SortDirectionReference, sortOrder);
            SortOrderReference = sortOrder;
            Messaging.UiUpdate.ReferenceTables.Send();
        }



        private static SortOrder SortOrderReference
        {
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private static SortDirection SortDirectionReference
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }

        private static SortOrder SortOrderAccounts
        {
            get => ApplicationSettings.SortOrderAccounts;
            set => ApplicationSettings.SortOrderAccounts = value;
        }

        private static SortDirection SortDirectionAccounts
        {
            get => ApplicationSettings.SortDirectionAccounts;
            set => ApplicationSettings.SortDirectionAccounts = value;
        }
    }
}