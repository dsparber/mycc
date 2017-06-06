using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.DataItems;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Ui.Get
{
    public class AccountDetailViewData
    {
        private static ITextResolver TextResolver => DependencyService.Get<ITextResolver>();

        public static HeaderDataItem HeaderData(Account account)
        {
            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { account.Money.Currency.Id })
                .Select(x => new Money(account.Money.Amount * MyccUtil.Rates.GetRate(new RateDescriptor(account.Money.Currency.Id, x))?.Rate ?? 0, x.Find())).
                OrderBy(m => m.Currency.Code);

            return new HeaderDataItem(account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney),
                additionalReferences.Any() ? string.Join(" / ", additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : account.Money.Currency.Name);
        }

        public string AccountName(Account account) => account.Name;

        public bool AddressClickable(Account account) => account is OnlineFunctionalAccount && !(account is BittrexAccount) && !(account is PoloniexAccount) && (!(account is BlockchainXpubAccount) || !ApplicationSettings.SecureXpub);

        public string AddressClickUrl(FunctionalAccount account) => (AccountStorage.RepositoryOf(account) as AddressAccountRepository)?.WebUrl;

        public string AccountType(Account account) =>
            account is LocalAccount ? TextResolver.ManuallyAdded :
            account is BittrexAccount ? string.Format(TextResolver.AddedWith, ConstantNames.Bittrex) :
            account is PoloniexAccount ? string.Format(TextResolver.AddedWith, ConstantNames.Poloniex) :
            TextResolver.AddressAdded;


        public string AccountSource(FunctionalAccount account) => AccountStorage.RepositoryOf(account)?.Description;

        public bool ShowAccountSource(FunctionalAccount account) => AccountStorage.RepositoryOf(account) is AddressAccountRepository;

        public string AccountAddressString(FunctionalAccount account) => account is BlockchainXpubAccount ? "xpub" : (AccountStorage.RepositoryOf(account) as AddressAccountRepository)?.Address.MiddleTruncate();

        public bool ShowAccountAddress(FunctionalAccount account) => AccountStorage.RepositoryOf(account) is AddressAccountRepository;

        public static DateTime LastUpdate(FunctionalAccount account)
        {
            var accountTime = account.LastUpdate;
            var ratesTime = MyccUtil.Rates.LastUpdateFor(account.Money.Currency.Id);

            return account is LocalAccount ? ratesTime : ratesTime < accountTime ? ratesTime : accountTime;
        }

        public static IEnumerable<ReferenceValueItem> Items(Account account)
        {
            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { account.Money.Currency.Id })
                .Select(c =>
                {
                    var rate = MyccUtil.Rates.GetRate(new RateDescriptor(account.Money.Currency.Id, c));
                    return new ReferenceValueItem(account.Money.Amount, rate?.Rate, c);
                })
                .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirection == SortDirection.Ascending);
        }

        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = TextResolver.Amount,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = TextResolver.Currency,
                SortDirection = SortDirectionHelper.GetSortDirection(SortOrder, SortDirection, SortOrder.Alphabetical),
                RightAligned = false,
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
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private static SortDirection SortDirection
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }
    }
}