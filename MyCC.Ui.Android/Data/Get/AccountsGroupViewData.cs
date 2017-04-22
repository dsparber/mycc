using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;
using MyCC.Core.Currency.Model;

namespace MyCC.Ui.Android.Data.Get
{
    public class AccountsGroupViewData
    {
        private readonly Context _context;

        public AccountsGroupViewData(Context context)
        {
            _context = context;
        }

        public static HeaderDataItem HeaderData(Currency currency)
        {
            var money = new Money(EnabledAccountsItems(currency).Sum(a => a.Money.Amount), currency);

            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { currency })
                .Select(x => new Money(money.Amount * ExchangeRateHelper.GetRate(currency, x)?.Rate ?? 0, x)).
                OrderBy(m => m.Currency.Code);

            return new HeaderDataItem(money.ToStringTwoDigits(ApplicationSettings.RoundMoney), string.Join(" / ", additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))));
        }


        public static IEnumerable<ReferenceValueItem> ReferenceItems(Currency currency)
        {
            var money = new Money(EnabledAccountsItems(currency).Sum(a => a.Money.Amount), currency);

            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { money.Currency })
                .Select(c => new ReferenceValueItem(money.Amount, ExchangeRateHelper.GetRate(money.Currency, c)))
                .OrderByWithDirection(c => SortOrderReference == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Value, SortDirectionReference == SortDirection.Ascending);
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
                Text = _context.Resources.GetString(Resource.String.Amount),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderReference, SortDirectionReference, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
                OnClick = () => OnSortReference(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Currency),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderReference, SortDirectionReference, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () => OnSortReference(SortOrder.Alphabetical)
            }
        };

        public List<SortButtonItem> SortButtonsAccounts => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Name),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderAccounts, SortDirectionAccounts, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () => OnSortAccounts(SortOrder.Alphabetical)
            },
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Amount),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrderAccounts, SortDirectionAccounts, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
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
            get { return ApplicationSettings.SortOrderReferenceValues; }
            set { ApplicationSettings.SortOrderReferenceValues = value; }
        }

        private static SortDirection SortDirectionReference
        {
            get { return ApplicationSettings.SortDirectionReferenceValues; }
            set { ApplicationSettings.SortDirectionReferenceValues = value; }
        }

        private static SortOrder SortOrderAccounts
        {
            get { return ApplicationSettings.SortOrderAccounts; }
            set { ApplicationSettings.SortOrderAccounts = value; }
        }

        private static SortDirection SortDirectionAccounts
        {
            get { return ApplicationSettings.SortDirectionAccounts; }
            set { ApplicationSettings.SortDirectionAccounts = value; }
        }
    }
}