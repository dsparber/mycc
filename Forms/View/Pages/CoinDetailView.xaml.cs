using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinDetailView
    {
        private readonly ReferenceCurrenciesView _referenceView;
        private readonly AccountsTableComponent _accountsView;

        private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> _accounts;

        private readonly Currency _currency;
        private Money MoneySum => (_accounts.ToList().Count == 0) ? null : new Money(_accounts.Where(a => a.Item1.IsEnabled).Sum(a => a.Item1.Money.Amount), _accounts.First().Item1.Money.Currency);

        public CoinDetailView(Currency pageCurrency)
        {
            InitializeComponent();

            var header = new CoinHeaderComponent(pageCurrency, true);
            ChangingStack.Children.Insert(0, header);

            _currency = pageCurrency;
            Title = _currency.Code;

            LoadData();

            _accountsView = new AccountsTableComponent(Navigation, _currency);

            ContentView.Children.Add(_accountsView);

            _referenceView = new ReferenceCurrenciesView(MoneySum);
            ContentView.Children.Add(_referenceView);

            Subscribe();
        }

        private void LoadData()
        {
            var accs = AccountStorage.Instance.AllElementsWithRepositories;
            _accounts = accs.Where(t => t.Item1.Money.Currency.Code.Equals(_currency.Code)).ToList();

            if (_accounts.ToList().Count == 0)
            {
                Navigation.RemovePage(this);
                return;
            }
            if (_referenceView == null) return;

            _referenceView.ReferenceMoney = MoneySum;
            _referenceView.UpdateView();
        }

        private void Subscribe()
        {
            Messaging.UpdatingAccounts.SubscribeFinished(this, LoadData);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, LoadData);
            Messaging.FetchMissingRates.SubscribeFinished(this, LoadData);

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, LoadData);
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, LoadData);
        }

        private async void Refresh(object sender, EventArgs args)
        {
            RefreshItem.Clicked -= Refresh;
            await AppTaskHelper.FetchBalanceAndRates(_currency);
            RefreshItem.Clicked += Refresh;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _accountsView.OnAppearing();
            _referenceView.OnAppearing();
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_currency));
        }
    }
}

