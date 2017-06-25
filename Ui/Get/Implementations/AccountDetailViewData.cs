using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
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

namespace MyCC.Ui.Get.Implementations
{
    internal class AccountDetailViewData : IAccountDetailViewData
    {
        private static ITextResolver TextResolver => DependencyService.Get<ITextResolver>();

        public HeaderItem HeaderData(int accountId)
        {
            var account = AccountStorage.GetAccount(accountId);
            if (account == null) return null;

            var additionalReferences = ApplicationSettings.MainCurrencies.Except(new[] { account.Money.Currency.Id })
                .Select(x => new Money(account.Money.Amount * MyccUtil.Rates.GetRate(new RateDescriptor(account.Money.Currency.Id, x))?.Rate ?? 0, x.Find())).
                OrderBy(m => m.Currency.Code);

            return new HeaderItem(account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney),
                additionalReferences.Any() ? string.Join(" / ", additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : account.Money.Currency.Name);
        }

        public string AccountName(int accountId) => AccountStorage.GetAccount(accountId)?.Name;

        public string CurrencyId(int accountId) => AccountStorage.CurrencyIdOf(accountId);

        public int RepositoryId(int accountId) => AccountStorage.RepositoryOf(accountId)?.Id ?? -1;

        public bool IsLocal(int accountId) => AccountStorage.RepositoryOf(accountId) is LocalAccountRepository;

        public bool ShowQrCodePossible(int accountId)
        {
            var repo = AccountStorage.RepositoryOf(accountId);
            return repo is OnlineAccountRepository && !(repo is BlockchainXpubAccountRepository);
        }

        public bool BlockExplorerCallAllowed(int accountId)
        {
            var repo = AccountStorage.RepositoryOf(accountId);
            return repo is OnlineAccountRepository && (!ApplicationSettings.SecureXpub || !(repo is BlockchainXpubAccountRepository));
        }

        public string AddressClickUrl(int accountId) => (AccountStorage.RepositoryOf(accountId) as AddressAccountRepository)?.WebUrl;

        public string ReferenceTableHeader(int accountId)
        {
            var money = AccountStorage.GetAccount(accountId)?.Money;
            if (money == null) return string.Empty;

            return string.Format(money.Amount == 1 ? StringUtils.TextResolver.IsEqualTo : StringUtils.TextResolver.AreEqualTo, money);
        }

        public string AccountType(int accountId)
        {
            var account = AccountStorage.GetAccount(accountId);
            return account is LocalAccount ? TextResolver.ManuallyAdded :
                   account is BittrexAccount ? string.Format(TextResolver.AddedWith, ConstantNames.Bittrex) :
                   account is PoloniexAccount ? string.Format(TextResolver.AddedWith, ConstantNames.Poloniex) :
                   TextResolver.AddressAdded;
        }

        public string AccountSource(int accountId) => AccountStorage.RepositoryOf(accountId)?.Description;

        public bool ShowAccountSource(int accountId) => AccountStorage.RepositoryOf(accountId) is AddressAccountRepository;

        public string AccountAddressString(int accountId)
        {
            var account = AccountStorage.GetAccount(accountId);
            return account is BlockchainXpubAccount ? "xpub" : (AccountStorage.RepositoryOf(accountId) as AddressAccountRepository)?.Address.MiddleTruncate();
        }

        public bool ShowAccountAddress(int accountId) => AccountStorage.RepositoryOf(accountId) is AddressAccountRepository;

        public DateTime LastUpdate(int accountId)
        {
            var account = AccountStorage.GetAccount(accountId);
            if (account == null) return DateTime.MinValue;

            var accountTime = (account as FunctionalAccount)?.LastUpdate ?? DateTime.MinValue;
            var ratesTime = MyccUtil.Rates.LastUpdateFor(account.Money.Currency.Id);

            return account is LocalAccount ? ratesTime : ratesTime < accountTime ? ratesTime : accountTime;
        }

        public IEnumerable<ReferenceValueItem> GetReferenceItems(int accountId)
        {
            var currencyId = CurrencyId(accountId);
            if (currencyId == null) return new List<ReferenceValueItem>();

            return ApplicationSettings.AllReferenceCurrencies.Except(new[] { currencyId })
            .Select(c =>
            {
                var rate = MyccUtil.Rates.GetRate(new RateDescriptor(currencyId, c));
                return new ReferenceValueItem(AccountStorage.GetAccount(accountId).Money.Amount, rate?.Rate, c);
            })
            .OrderByWithDirection(c => SortOrder == SortOrder.Alphabetical ? c.CurrencyCode as object : c.Rate, SortDirection == SortDirection.Ascending);
        }

        public List<SortButtonItem> SortButtons => new List<SortButtonItem>
        {
            new SortButtonItem
            {
                Text = TextResolver.Amount,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.ByValue),
                RightAligned = true,
                OnClick = () => OnSort(SortOrder.ByValue)
            },
            new SortButtonItem
            {
                Text = TextResolver.Currency,
                SortAscending = SortDirectionHelper.GetSortAscending(SortOrder, SortDirection, SortOrder.Alphabetical),
                RightAligned = false,
                OnClick = () => OnSort(SortOrder.Alphabetical)
            }
        };

        private void OnSort(SortOrder sortOrder)
        {
            SortDirection = SortDirectionHelper.GetNewSortDirection(SortOrder, SortDirection, sortOrder);
            SortOrder = sortOrder;
            Messaging.Update.Balances.Send();
        }

        private SortOrder SortOrder
        {
            get => ApplicationSettings.SortOrderReferenceValues;
            set => ApplicationSettings.SortOrderReferenceValues = value;
        }

        private SortDirection SortDirection
        {
            get => ApplicationSettings.SortDirectionReferenceValues;
            set => ApplicationSettings.SortDirectionReferenceValues = value;
        }
    }
}