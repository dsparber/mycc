using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using MyCryptos.view.components;
using System;
using MyCryptos.Core.Helpers;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;

namespace view
{
    public partial class CoinDetailView : ContentPage
    {
        List<AccountViewCell> Cells;
        List<ReferenceValueViewCell> ReferenceValueCells;

        List<ExchangeRate> ExchangeRates;
        IEnumerable<Tuple<Account, AccountRepository>> Accounts;

        Currency currency;
        Money moneySum { get { return (Accounts.ToList().Count == 0) ? null : new Money(Accounts.Sum(a => a.Item1.Money.Amount), Accounts.First().Item1.Money.Currency); } }

        public CoinDetailView(Currency pageCurrency)
        {
            InitializeComponent();

            currency = pageCurrency;

            subscribe();
            loadData();
        }

        void loadData()
        {
            var accs = AccountStorage.Instance.AllElementsWithRepositories;
            Accounts = accs.Where(t => t.Item1.Money.Currency.Equals(currency)).ToList();

            var currencies = ApplicationSettings.ReferenceCurrencies;

            ExchangeRates = new List<ExchangeRate>();
            foreach (var c in currencies)
            {
                ExchangeRates.Add(ExchangeRateHelper.GetRate(currency, c));
            }
            ExchangeRates.RemoveAll(e => e == null);


            if (Accounts.ToList().Count == 0)
            {
                Navigation.RemovePage(this);
            }
            else
            {
                updateView();
            }
        }

        void updateView()
        {
            Cells = new List<AccountViewCell>();
            foreach (var a in Accounts)
            {
                Cells.Add(new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });
            }

            var table = new ReferenceCurrenciesSection(moneySum);
            ReferenceValueCells = table.Cells;
            EqualsSection.Clear();
            foreach (var c in ReferenceValueCells)
            {
                EqualsSection.Add(c);
            }

            SortHelper.ApplySortOrder(Cells, AccountSection);

            var neededRates = ReferenceValueCells.Where(c => c.IsLoading).Select(c => c.ExchangeRate);

            Messaging.UpdatingExchangeRates.SendStarted();
            var task = ApplicationTasks.FetchMissingRates(neededRates);
            task.ContinueWith(t => Messaging.UpdatingExchangeRates.SendFinished());

            setHeader();
        }

        void setHeader()
        {
            Title = currency.Code;
            Header.TitleText = moneySum.ToString();

            var exchangeRate = ExchangeRates.Find(e => e.SecondaryCurrency.Equals(ApplicationSettings.BaseCurrency));

            if (exchangeRate != null && exchangeRate.Rate.HasValue)
            {
                var moneyReference = new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
                Header.InfoText = moneyReference.ToString();
            }
            else
            {
                Header.InfoText = I18N.NoExchangeRateFound;

            }
        }

        void subscribe()
        {
            Messaging.SortOrder.SubscribeValueChanged(this, () =>
            {
                SortHelper.ApplySortOrder(Cells, AccountSection);
                SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);
            });

            Messaging.UpdatingAccounts.SubscribeFinished(this, loadData);
            Messaging.UpdatingExchangeRates.SubscribeFinished(this, loadData);

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, loadData);
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, loadData);
        }
    }
}
