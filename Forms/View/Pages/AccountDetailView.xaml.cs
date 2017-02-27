using System;
using System.Dynamic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.view.components.CellViews;
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
            var repo = AccountStorage.RepositoryOf(_account);

            if (repo is AddressAccountRepository && !(repo is BlockchainXpubAccountRepository))
            {
                var qrButton = new ToolbarItem { Icon = "qr.png" };
                qrButton.Clicked += (sender, e) => Navigation.PushOrPushModal(new AccountQrCodeOverlay((AddressAccountRepository)repo));
                ToolbarItems.Add(qrButton);
            }

            var infoStackContainer = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = new Thickness(15, 0) };
            var headerStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
            var dataStack = new StackLayout();


            headerStack.Children.Add(new Label { Text = $"{I18N.Name}:", FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.FontColorLight });

            var nameLabel = new Label
            {
                Text = _account.Name,
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            dataStack.Children.Add(nameLabel);

            headerStack.Children.Add(new Label { Text = $"{I18N.Source}:", FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.FontColorLight });
            dataStack.Children.Add(new Label { Text = repo is LocalAccountRepository ? I18N.ManuallyAdded : repo.Description, FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.FontColorLight, LineBreakMode = LineBreakMode.TailTruncation });

            Action updateLabelAction = () => nameLabel.Text = _account.Name;

            if (repo is AddressAccountRepository)
            {
                headerStack.Children.Add(new Label { Text = $"{I18N.Address}:", FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.FontColorLight });
                var addressLabel = new Label
                {
                    Text = (repo as AddressAccountRepository).Address,
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight,
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                dataStack.Children.Add(addressLabel);
                updateLabelAction = () =>
                {
                    nameLabel.Text = _account.Name;
                    addressLabel.Text = (repo as AddressAccountRepository)?.Address ?? string.Empty;
                };
            }

            infoStackContainer.Children.Add(headerStack);
            infoStackContainer.Children.Add(dataStack);
            stack.Children.Add(new SectionHeaderView(false) { Title = I18N.Info });
            stack.Children.Add(infoStackContainer);
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
            SetFooter();

            Messaging.Progress.SubscribeToComplete(this, SetFooter);
            Messaging.UpdatingAccounts.SubscribeFinished(this, updateLabelAction);
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

        private void SetFooter()
        {
            var accountTime = _account.LastUpdate;
            var ratesTime = AccountStorage.NeededRatesFor(_account).Distinct().Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            var time = _account is LocalAccount ? ratesTime : ratesTime < accountTime ? ratesTime : accountTime;

            Device.BeginInvokeOnMainThread(() => Footer.Text = time.LastUpdateString());
        }
    }
}

