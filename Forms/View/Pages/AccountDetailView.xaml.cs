using System;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Overlays;
using MyCC.Forms.View.Pages.Settings;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class AccountDetailView
    {
        private readonly FunctionalAccount _account;
        private readonly ReferenceCurrenciesView _referenceView;
        private readonly PullToRefreshLayout _pullToRefresh;


        public AccountDetailView(FunctionalAccount account)
        {
            InitializeComponent();

            _account = account;

            var header = new CoinHeaderComponent(account);
            ChangingStack.Children.Insert(0, header);
            _referenceView = new ReferenceCurrenciesView(account.Money);

            var stack = new StackLayout { Spacing = 0, Margin = new Thickness(0, 0, 0, 40) };
            var repo = AccountStorage.Instance.Repositories.Find(r => r.Id == account.ParentId);

            if (repo is AddressAccountRepository && !(repo is BlockchainXpubAccountRepository))
            {
                var qrButton = new ToolbarItem { Icon = "qr.png" };
                qrButton.Clicked += (sender, e) => Navigation.PushOrPushModal(new AccountQrCodeOverlay((AddressAccountRepository)repo));
                ToolbarItems.Add(qrButton);
            }

            stack.Children.Add(_referenceView);


            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { Content = stack },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh),
            };

            ContentView.Content = _pullToRefresh;

            SetView();
        }

        private void SetView()
        {
            Title = _account.Money.Currency.Code;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _referenceView.OnAppearing();
        }

        private async void Refresh()
        {
            await AppTaskHelper.FetchBalanceAndRates(_account);
            _pullToRefresh.IsRefreshing = false;
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_account.Money.Currency));
        }

        private void Edit(object sender, EventArgs args)
        {
            var repo = AccountStorage.RepositoryOf(_account);
            if (repo is LocalAccountRepository)
            {
                Navigation.PushOrPushModal(new AccountEditView(_account, repo as LocalAccountRepository, true));
            }
            else
            {
                Navigation.PushOrPushModal(new RepositoryView(repo, true));
            }
        }
    }
}

