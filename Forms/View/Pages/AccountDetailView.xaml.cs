using System;
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
using MyCC.Forms.View.Components.CellViews;
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

            var infoStackContainer = new StackLayout { Spacing = 30, Orientation = StackOrientation.Horizontal, Margin = new Thickness(15, 0) };
            var headerStack = new StackLayout();
            var dataStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };

            headerStack.Children.Add(new Label
            {
                Text = $"{I18N.Name}:",
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight,
                LineBreakMode = LineBreakMode.NoWrap
            });

            var nameLabel = new Label
            {
                Text = _account.Name,
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight,
                LineBreakMode = LineBreakMode.MiddleTruncation
            };
            dataStack.Children.Add(nameLabel);

            headerStack.Children.Add(new Label
            {
                LineBreakMode = LineBreakMode.NoWrap,
                Text = $"{I18N.Type}:",
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight
            });
            dataStack.Children.Add(new Label
            {
                Text = repo is LocalAccountRepository ? I18N.ManuallyAdded : repo is AddressAccountRepository ? I18N.AddressAdded : I18N.BittrexAdded,
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight,
                LineBreakMode = LineBreakMode.MiddleTruncation
            });

            Action updateLabelAction = () => nameLabel.Text = _account.Name;

            if (repo is AddressAccountRepository)
            {
                Func<string, string> shorten = address =>
                {
                    if (string.IsNullOrWhiteSpace(address)) return string.Empty;

                    var firstNumbers = address.Length >= 5 ? address.Substring(0, 5) : address.Substring(0, address.Length - 1);
                    var lastNumbers = address.Length >= 10 ? address.Substring(address.Length - 5, 5) : string.Empty;
                    return $"{firstNumbers}...{lastNumbers}";
                };

                headerStack.Children.Add(new Label
                {
                    LineBreakMode = LineBreakMode.NoWrap,
                    Text = $"{I18N.Source}:",
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight
                });
                var sourceLabel = new Label
                {
                    Text = repo.Description,
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight,
                    LineBreakMode = LineBreakMode.MiddleTruncation
                };
                dataStack.Children.Add(sourceLabel);

                headerStack.Children.Add(new Label
                {
                    LineBreakMode = LineBreakMode.NoWrap,
                    Text = $"{I18N.Address}:",
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight
                });
                var addressLabel = new Label
                {
                    Text = shorten((repo as AddressAccountRepository).Address),
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight,
                    LineBreakMode = LineBreakMode.MiddleTruncation
                };
                dataStack.Children.Add(addressLabel);
                updateLabelAction = () =>
                {
                    nameLabel.Text = _account.Name;
                    addressLabel.Text = shorten((repo as AddressAccountRepository)?.Address);
                    sourceLabel.Text = repo.Description;
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

            if (!AccountStorage.Instance.AllElements.Contains(_account))
            {
                Navigation.PopAsync();
            }
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

