using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Data.Get
{
    public class AccountDetailViewData
    {
        private readonly Context _context;

        public AccountDetailViewData(Context context)
        {
            _context = context;
        }

        public static HeaderDataItem HeaderData(Account account)
        {
            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { account.Money.Currency })
                .Select(x => new Money(account.Money.Amount * ExchangeRateHelper.GetRate(account.Money.Currency, x)?.Rate ?? 0, x)).
                OrderBy(m => m.Currency.Code);

            return new HeaderDataItem(account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney),
                string.Join(" / ", additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))));
        }

        public string AccountName(Account account) => account.Name;

        public bool AddressClickable(Account account) => account is OnlineFunctionalAccount && !(account is BittrexAccount) && !(account is BlockchainXpubAccount);

        public string AddressClickUrl(FunctionalAccount account) => (AccountStorage.RepositoryOf(account) as AddressAccountRepository)?.WebUrl;

        public string AccountType(Account account)
            => _context.Resources.GetString(account is LocalAccount ? Resource.String.ManuallyAdded :
                    account is BittrexAccount ? Resource.String.BittrexAdded :
                    Resource.String.AddressAdded);

        public string AccountSource(FunctionalAccount account) => AccountStorage.RepositoryOf(account)?.Description;

        public bool ShowAccountSource(FunctionalAccount account) => AccountStorage.RepositoryOf(account) is AddressAccountRepository;

        public string AccountAddressString(FunctionalAccount account) => account is BlockchainXpubAccount ? "xpub" : (AccountStorage.RepositoryOf(account) as AddressAccountRepository)?.Address.MiddleTruncate();

        public bool ShowAccountAddress(FunctionalAccount account) => AccountStorage.RepositoryOf(account) is AddressAccountRepository;

        public static DateTime LastUpdate(FunctionalAccount account)
        {
            var accountTime = account.LastUpdate;
            var ratesTime = AccountStorage.NeededRatesFor(account).Distinct().Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            return account is LocalAccount ? ratesTime : ratesTime < accountTime ? ratesTime : accountTime;
        }

        public static IEnumerable<ReferenceValueItem> Items(Account account)
        {
            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { account.Money.Currency })
                .Select(c => new ReferenceValueItem(account.Money.Amount, ExchangeRateHelper.GetRate(account.Money.Currency, c)))
                .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Value, SortDirection == SortDirection.Ascending);
        }

        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Amount),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
                TextGravity = GravityFlags.Right,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = _context.Resources.GetString(Resource.String.Currency),
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                TextGravity = GravityFlags.Left,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            }
        };

        private static void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            Messaging.UiUpdate.ReferenceTables.Send();
        }



        private static SortOrder SortOrder
        {
            get { return ApplicationSettings.SortOrderReferenceValues; }
            set { ApplicationSettings.SortOrderReferenceValues = value; }
        }

        private static SortDirection SortDirection
        {
            get { return ApplicationSettings.SortDirectionReferenceValues; }
            set { ApplicationSettings.SortDirectionReferenceValues = value; }
        }
    }
}