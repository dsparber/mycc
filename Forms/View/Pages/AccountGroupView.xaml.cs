using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Rates;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using Plugin.Connectivity;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class AccountGroupView
    {
        private readonly ReferenceCurrenciesView _referenceView;
        private readonly AccountsTableComponent _accountsViewEnabled;
        private readonly AccountsTableComponent _accountsViewDisabled;
        private readonly PullToRefreshLayout _pullToRefresh;

        private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> _accounts;

        private readonly Currency _currency;
        private Money MoneySum => _accounts.ToList().Count == 0 ? null : new Money(_accounts.Where(a => a.Item1.IsEnabled).Sum(a => a.Item1.Money.Amount), _accounts.First().Item1.Money.Currency);

        public AccountGroupView(Currency pageCurrency)
        {
            InitializeComponent();

            var header = new CoinHeaderComponent(pageCurrency, true);
            ChangingStack.Children.Insert(0, header);

            _currency = pageCurrency;
            Title = $"\u03A3 {_currency.Code}";

            LoadData();

            _accountsViewEnabled = new AccountsTableComponent(Navigation, _currency, true);
            _accountsViewDisabled = new AccountsTableComponent(Navigation, _currency, false);

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(_accountsViewEnabled);
            _referenceView = new ReferenceCurrenciesView(MoneySum);

            stack.Children.Add(_referenceView);
            stack.Children.Add(_accountsViewDisabled);

            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { Content = stack },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh),
            };

            ContentView.Content = _pullToRefresh;

            Subscribe();
            SetFooter();
        }

        private void LoadData()
        {
            var accs = AccountStorage.Instance.AllElementsWithRepositories;
            _accounts = accs.Where(t => t.Item1.Money.Currency.Code.Equals(_currency.Code)).ToList();

            if (_accounts.ToList().Count == 0) return;


            SetFooter();
            Device.BeginInvokeOnMainThread(() =>
            {
                _accountsViewEnabled.IsVisible = _accounts.Any(a => a.Item1.IsEnabled);
                _accountsViewDisabled.IsVisible = _accounts.Any(a => !a.Item1.IsEnabled);
            });


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

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await AppTaskHelper.FetchBalanceAndRates(_currency);
                _pullToRefresh.IsRefreshing = false;
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_accounts.ToList().Count == 0)
            {
                Navigation.PopAsync();
            }
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_currency));
        }

        private void SetFooter()
        {
            var online = AccountStorage.AccountsWithCurrency(_currency).Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.AccountsWithCurrency(_currency).Select(a => a.LastUpdate).DefaultIfEmpty(DateTime.Now).Max();
            var ratesTime = AccountStorage.NeededRatesFor(_currency).Distinct().Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            var time = online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;

            Device.BeginInvokeOnMainThread(() => Footer.Text = time.LastUpdateString());
        }

        private void AddSource(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceOverlay());
        }
    }
}

