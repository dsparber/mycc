using System;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class AccountDetailView
    {
        private readonly FunctionalAccount _account;
        private readonly CoinHeaderComponent _header;
        private readonly ReferenceCurrenciesView _referenceView;

        public AccountDetailView(FunctionalAccount account)
        {
            InitializeComponent();

            _account = account;

            _header = new CoinHeaderComponent(account);
            ChangingStack.Children.Insert(0, _header);
            _referenceView = new ReferenceCurrenciesView(account.Money);

            var stack = new StackLayout { Spacing = 40, Margin = new Thickness(0, 40) };
            var repo = AccountStorage.Instance.Repositories.Find(r => r.Id == account.ParentId);

            var disableCell = new CustomSwitchView
            {
                On = !_account.IsEnabled,
                Title = I18N.DisableAccount,
                Info = I18N.DisabledInOverview
            };
            disableCell.Switch.Toggled += async (sender, args) =>
            {
                _account.IsEnabled = !disableCell.On;
                await AccountStorage.Update(_account);
                Messaging.UpdatingAccounts.SendFinished();
            };

            if (repo is AddressAccountRepository && !(repo is BlockchainXpubAccountRepository))
            {
                var qrButton = new Button
                {
                    Text = I18N.ShowQrCode,
                    BackgroundColor = Color.White,
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.ThemeColor,
                    BorderWidth = 0.5,
                    BorderColor = Color.FromHex("#ccc"),
                    BorderRadius = 0
                };
                qrButton.Clicked += (sender, e) => Navigation.PushOrPushModal(new AccountQrCodeOverlay((AddressAccountRepository)repo));
                stack.Children.Add(qrButton);
            }

            stack.Children.Add(disableCell);
            stack.Children.Add(_referenceView);


            ContentView.Content = stack;

            SetView();

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => Update());
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, () => Update());

            Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update());
            Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Update(true), () => Update());
            Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update());
        }

        private void SetView()
        {
            Title = _account.Money.Currency.Code;
            Update();
        }

        private void Update(bool loading = false)
        {
            Device.BeginInvokeOnMainThread(() => _header.IsLoading = loading);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _referenceView.OnAppearing();
        }

        private async void Refresh(object sender, EventArgs args)
        {
            RefreshItem.Clicked -= Refresh;
            if (_account is OnlineFunctionalAccount)
            {
                await AppTaskHelper.FetchBalanceAndRates((OnlineFunctionalAccount)_account);
            }
            await AppTaskHelper.FetchMissingRates();
            RefreshItem.Clicked += Refresh;
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_account.Money.Currency));
        }
    }
}

