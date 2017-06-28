using System;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
using MyCC.Ui.Messages;
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
        private readonly HeaderView _headerView;


        private readonly string _currencyId;

        public AccountGroupView(string pageCurrencyId)
        {
            InitializeComponent();

            _headerView = new HeaderView(true);
            ChangingStack.Children.Insert(0, _headerView);

            _currencyId = pageCurrencyId;
            Title = $"\u03A3 {_currencyId.Code()}";

            Update();

            _accountsViewEnabled = new AccountsTableComponent(Navigation, _currencyId, true);
            _accountsViewDisabled = new AccountsTableComponent(Navigation, _currencyId, false);

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(_accountsViewEnabled);
            _referenceView = new ReferenceCurrenciesView
            {
                Items = (UiUtils.Get.AccountsGroup.ReferenceItems(_currencyId), UiUtils.Get.AccountsGroup.SortButtonsReference),
                Title = UiUtils.Get.AccountsGroup.ReferenceTableHeader(_currencyId)
            };


            stack.Children.Add(_referenceView);
            stack.Children.Add(_accountsViewDisabled);

            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { Content = stack },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh)
            };

            ContentView.Content = _pullToRefresh;

            Subscribe();
            SetFooter();
        }

        private void Update()
        {
            SetFooter();
            Device.BeginInvokeOnMainThread(() =>
            {
                _headerView.Data = UiUtils.Get.AccountsGroup.HeaderData(_currencyId);
                _accountsViewEnabled.IsVisible = UiUtils.Get.AccountsGroup.EnabledAccountsItems(_currencyId).Any();
                _accountsViewDisabled.IsVisible = UiUtils.Get.AccountsGroup.DisabledAccountsItems(_currencyId).Any();
            });


            if (_referenceView == null) return;

            _referenceView.Items = (UiUtils.Get.AccountsGroup.ReferenceItems(_currencyId), UiUtils.Get.AccountsGroup.SortButtonsReference);
            _referenceView.Title = UiUtils.Get.AccountsGroup.ReferenceTableHeader(_currencyId);
        }

        private void Subscribe()
        {
            Messaging.Update.Balances.Subscribe(this, Update);
            Messaging.Update.Rates.Subscribe(this, Update);
            Messaging.Modified.Balances.Subscribe(this, Update);
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, Update);
            Messaging.Sort.ReferenceTables.Subscribe(this, Update);
            Messaging.Status.Progress.SubscribeFinished(this, () => _pullToRefresh.IsRefreshing = false);
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchBalancesAndRatesFor(_currencyId);
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

            if (!UiUtils.Get.AccountsGroup.HasAccounts(_currencyId))
            {
                Navigation.PopAsync();
            }
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_currencyId));
        }

        private void SetFooter()
        {
            Device.BeginInvokeOnMainThread(() => Footer.Text = UiUtils.Get.AccountsGroup.LastUpdate(_currencyId).LastUpdateString());
        }

        private void AddSource(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceOverlay());
        }
    }
}

