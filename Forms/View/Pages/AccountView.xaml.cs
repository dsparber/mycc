using System;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using MyCC.Forms.View.Pages.Settings.Source;
using MyCC.Ui;
using MyCC.Ui.Messages;
using Plugin.Connectivity;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class AccountView
    {
        private FunctionalAccount _account;
        private readonly PullToRefreshLayout _pullToRefresh;


        public AccountView(FunctionalAccount account)
        {
            InitializeComponent();

            _account = account;

            var header = new HeaderView(true) { Data = UiUtils.Get.AccountDetail.HeaderData(_account.Id) };
            ChangingStack.Children.Insert(0, header);

            var referenceView = new ReferenceCurrenciesView
            {
                Items = (UiUtils.Get.AccountDetail.GetReferenceItems(_account.Id), UiUtils.Get.AccountDetail.SortButtons),
                Title = UiUtils.Get.AccountDetail.ReferenceTableHeader(_account.Id)
            };

            var stack = new StackLayout { Spacing = 0, Margin = new Thickness(0, 0, 0, 40), Padding = new Thickness(0, 40, 0, 40) };
            var repo = AccountStorage.RepositoryOf(_account);

            if (repo is AddressAccountRepository && !(repo is BlockchainXpubAccountRepository))
            {
                var qrButton = new ToolbarItem { Icon = "qr.png" };
                qrButton.Clicked +=
                    (sender, e) => Navigation.PushOrPushModal(new AccountQrCodeOverlay((AddressAccountRepository)repo));
                ToolbarItems.Add(qrButton);
            }

            var infoStackContainer = new StackLayout
            {
                Spacing = 30,
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(15, 0)
            };
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
                Text = repo is LocalAccountRepository ? I18N.ManuallyAdded : repo is AddressAccountRepository ? I18N.AddressAdded : string.Format(I18N.AddedWith, repo is BittrexAccountRepository ? ConstantNames.Bittrex : ConstantNames.Poloniex),
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.FontColorLight,
                LineBreakMode = LineBreakMode.MiddleTruncation
            });

            Action updateLabelAction = () => nameLabel.Text = _account.Name;

            if (repo is AddressAccountRepository)
            {
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
                    Text = repo is BlockchainXpubAccountRepository ? "xpub" : (repo as AddressAccountRepository).Address.MiddleTruncate(),
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight,
                    LineBreakMode = LineBreakMode.MiddleTruncation,
                };

                dataStack.Children.Add(addressLabel);
                updateLabelAction = () =>
                {
                    nameLabel.Text = _account?.Name ?? repo.Name;
                    addressLabel.Text = repo is BlockchainXpubAccountRepository ? "xpub" : (repo as AddressAccountRepository)?.Address.MiddleTruncate() ?? string.Empty;
                    sourceLabel.Text = repo.Description;
                };
            }
            else if (_account is LocalAccount)
            {
                headerStack.Children.Add(new Label
                {
                    LineBreakMode = LineBreakMode.NoWrap,
                    Text = $"{I18N.LastChange}:",
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight
                });
                var lastChangeLabel = new Label
                {
                    Text = _account.LastUpdate.AsString(),
                    FontSize = AppConstants.TableSectionFontSize,
                    TextColor = AppConstants.FontColorLight,
                    LineBreakMode = LineBreakMode.MiddleTruncation
                };
                dataStack.Children.Add(lastChangeLabel);

                updateLabelAction = () =>
                {
                    nameLabel.Text = _account?.Name ?? repo.Name;
                    lastChangeLabel.Text = _account.LastUpdate.AsString();
                };
            }

            infoStackContainer.Children.Add(headerStack);
            infoStackContainer.Children.Add(dataStack);

            if (repo is AddressAccountRepository && (!(repo is BlockchainXpubAccountRepository) || !ApplicationSettings.SecureXpub))
            {
                var explorerButton = new CustomCellView(true) { IsActionCell = true, Text = I18N.OpenInBlockExplorer, IsCentered = true };
                var explorerGesture = new TapGestureRecognizer();
                explorerGesture.Tapped += (sender, args) =>
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Navigation.PushModalAsync(
                            new NavigationPage(new WebOverlay(((AddressAccountRepository)repo).WebUrl)));
                    }
                    else
                    {
                        DisplayAlert(I18N.Error, I18N.NoInternetAccess, I18N.Ok);
                    }
                };
                explorerButton.GestureRecognizers.Add(explorerGesture);

                stack.Children.Add(explorerButton);
            }
            stack.Children.Add(new SectionHeaderView(false) { Title = I18N.Info });
            stack.Children.Add(infoStackContainer);
            stack.Children.Add(referenceView);


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

            void Update()
            {
                if (_account != null && !(_account is LocalAccount))
                {
                    _account = AccountStorage.Instance.Repositories.Find(r => r.Id == _account.ParentId)?.Elements.FirstOrDefault(e => e.Money.Currency.Equals(_account.Money.Currency));
                }

                if (_account == null)
                {
                    try
                    {
                        Navigation.PopAsync();
                    }
                    catch
                    {
                        /* ignored */
                    }
                    return;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    header.Data = UiUtils.Get.AccountDetail.HeaderData(_account.Id);
                    updateLabelAction();
                    SetFooter();
                });
                referenceView.Items = (UiUtils.Get.AccountDetail.GetReferenceItems(_account.Id), UiUtils.Get.AccountDetail.SortButtons);
                referenceView.Title = UiUtils.Get.AccountDetail.ReferenceTableHeader(_account.Id);
            }

            Messaging.Update.Rates.Subscribe(this, Update);
            Messaging.Update.Balances.Subscribe(this, Update);
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, Update);
            Messaging.Modified.Balances.Subscribe(this, Update);
            Messaging.Sort.ReferenceTables.Subscribe(this, Update);
            Messaging.Status.Progress.SubscribeFinished(this, () => Device.BeginInvokeOnMainThread(() => _pullToRefresh.IsRefreshing = false));
        }

        private void SetView()
        {
            Title = _account.Money.Currency.Code;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!AccountStorage.Instance.AllElements.Contains(_account))
            {
                Navigation.PopToRootAsync();
            }
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchBalanceAndRatesFor(_account.Id);
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        private void ShowInfo(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CoinInfoView(_account.Money.Currency.Id));
        }

        private void Edit(object sender, EventArgs args)
        {
            var repo = AccountStorage.RepositoryOf(_account);
            if (repo is LocalAccountRepository)
            {
                Navigation.PushOrPushModal(new AccountEditView(_account, true));
            }
            else
            {
                Navigation.PushOrPushModal(new RepositoryView(repo as OnlineAccountRepository, true));
            }
        }

        private void SetFooter()
        {
            Footer.Text = UiUtils.Get.AccountDetail.LastUpdate(_account.Id).LastUpdateString();
        }
    }
}

