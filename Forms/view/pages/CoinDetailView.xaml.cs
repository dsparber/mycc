using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.view.components;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
    public partial class CoinDetailView : ContentPage
    {
        private readonly CoinsHeaderView Header;

        List<AccountViewCell> Cells;
        List<ReferenceValueViewCell> ReferenceValueCells;

        List<ExchangeRate> ExchangeRates;
        IEnumerable<Tuple<FunctionalAccount, AccountRepository>> Accounts;

        Currency currency;
        Money moneySum { get { return (Accounts.ToList().Count == 0) ? null : new Money(Accounts.Sum(a => a.Item1.Money.Amount), Accounts.First().Item1.Money.Currency); } }

        public CoinDetailView(Currency pageCurrency)
        {
            InitializeComponent();

            Header = new CoinsHeaderView(pageCurrency, true);
            ChangingStack.Children.Insert(0, Header);

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

            var neededRates = ReferenceValueCells.Where(c => c.IsLoading).Select(c => c.ExchangeRate).ToList();

            if (neededRates.Count > 0)
            {
                ApplicationTasks.FetchMissingRates(neededRates, Messaging.FetchMissingRates.SendStarted, Messaging.FetchMissingRates.SendFinished, ErrorOverlay.Display);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                EqualsSection.Clear();
                foreach (var c in ReferenceValueCells)
                {
                    EqualsSection.Add(c);
                }

                SortHelper.ApplySortOrder(Cells, AccountSection);
            });
        }

        void subscribe()
        {
            Messaging.SortOrder.SubscribeValueChanged(this, () =>
            {
                SortHelper.ApplySortOrder(Cells, AccountSection);
                SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);
            });

            Messaging.UpdatingAccounts.SubscribeFinished(this, loadData);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, loadData);
            Messaging.FetchMissingRates.SubscribeFinished(this, loadData);

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, loadData);
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, loadData);
        }

        private async void Refresh(object sender, EventArgs args)
        {
            await ApplicationTasks.FetchBalanceAndRates(currency, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
        }
    }
}

