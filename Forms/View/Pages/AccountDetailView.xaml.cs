using System;
using MyCC.Core.Account.Models.Base;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.view.components;
using Xamarin.Forms;
using MyCC.Forms.Resources;
using MyCC.Forms.helpers;
using MyCC.Forms.view.overlays;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.constants;

namespace MyCC.Forms.view.pages
{
    public partial class AccountDetailView
    {
        private FunctionalAccount account;
        private readonly CoinHeaderComponent header;
        private ReferenceCurrenciesView referenceView;

        public AccountDetailView(FunctionalAccount account)
        {
            InitializeComponent();

            this.account = account;

            header = new CoinHeaderComponent(account);
            ChangingStack.Children.Insert(0, header);
            referenceView = new ReferenceCurrenciesView(account.Money);

            var stack = new StackLayout { Spacing = 0 };
            var repo = AccountStorage.Instance.Repositories.Find(r => r.Id == account.ParentId);

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
                    Margin = new Thickness(0, 30, 0, 0),
                    BorderRadius = 0
                };
                qrButton.Clicked += (sender, e) => Navigation.PushOrPushModal(new AccountQrCodeOverlay((AddressAccountRepository)repo));
                stack.Children.Add(qrButton);
            }
            stack.Children.Add(referenceView);


            ContentView.Content = stack;

            SetView();

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => Update());
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, () => Update());

            Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));
            Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));
            Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));
        }

        private void SetView()
        {
            Title = account.Money.Currency.Code;
            Update();
        }

        private void Update(bool loading = false)
        {
            Device.BeginInvokeOnMainThread(() => header.IsLoading = loading);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            referenceView.OnAppearing();
        }

        private async void Refresh(object sender, EventArgs args)
        {
            RefreshItem.Clicked -= Refresh;
            if (account is OnlineFunctionalAccount)
            {
                await AppTaskHelper.FetchBalanceAndRates((OnlineFunctionalAccount)account);
            }
            await AppTaskHelper.FetchMissingRates();
            RefreshItem.Clicked += Refresh;
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(account.Money.Currency));
        }
    }
}

