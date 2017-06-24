using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using Xamarin.Forms;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
using MyCC.Ui.Messages;
using Plugin.Connectivity;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinInfoView
    {
        private readonly string _currencyId;
        private readonly ReferenceCurrenciesView _referenceView;


        private readonly Dictionary<string, Tuple<Label, Label>> _infos;

        public CoinInfoView(string currencyId, bool enableAccountButton = false)
        {
            InitializeComponent();

            _currencyId = currencyId;

            if (!enableAccountButton || !AccountStorage.AccountsWithCurrency(_currencyId).Any())
            {
                ToolbarItems.Remove(AccountsButton);
            }

            Title = _currencyId.Code();
            var header = new HeaderView { Data = UiUtils.Get.CoinInfo.GetHeaderData(_currencyId) };
            ChangingStack.Children.Insert(0, header);
            InfoHeading.Text = Device.RuntimePlatform.Equals(Device.iOS) ? I18N.Info.ToUpper() : I18N.Info;
            InfoHeading.TextColor = AppConstants.TableSectionColor;

            var allExplorer = CoinInfoStorage.Instance.GetExplorer(_currencyId).ToList();
            foreach (var e in allExplorer)
            {
                var explorerButton = new CustomCellView(true)
                {
                    IsActionCell = true,
                    Text = allExplorer.Count == 1 ? I18N.OpenInBlockExplorer : $"{I18N.OpenInBlockExplorer} ({e.Name})",
                    IsCentered = true
                };
                var explorerGesture = new TapGestureRecognizer();
                explorerGesture.Tapped += (sender, args) =>
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Navigation.PushModalAsync(new NavigationPage(new WebOverlay(e.WebUrl(_currencyId))));
                    }
                    else
                    {
                        DisplayAlert(I18N.Error, I18N.NoInternetAccess, I18N.Ok);
                    }
                };
                explorerButton.GestureRecognizers.Add(explorerGesture);

                ContentView.Children.Add(explorerButton);
            }

            _referenceView = new ReferenceCurrenciesView
            {
                Items = (UiUtils.Get.CoinInfo.ReferenceValues(_currencyId), UiUtils.Get.CoinInfo.SortButtons)
            };
            ContentView.Children.Add(_referenceView);

            Label GetLabel() => new Label { IsVisible = false, FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.TableSectionColor, LineBreakMode = LineBreakMode.TailTruncation };
            Tuple<Label, Label> GetTuple() => Tuple.Create(GetLabel(), GetLabel());

            _infos = new Dictionary<string, Tuple<Label, Label>> {
                {I18N.CurrencyCode, GetTuple()},
                {I18N.Name, GetTuple()},

                {I18N.CoinExplorer, GetTuple()},
                {I18N.CoinAlgorithm, GetTuple()},
                {I18N.Type, GetTuple()},

                {I18N.CoinHashrate, GetTuple()},
                {I18N.CoinDifficulty, GetTuple()},
                {I18N.CoinBlockReward, GetTuple()},
                {I18N.CoinBlockHeight, GetTuple()},
                {I18N.CoinBlocktime, GetTuple()},
                {I18N.CoinSupplyMax, GetTuple()},
                {I18N.CoinSupply, GetTuple()},
                {I18N.CoinMarketCap, GetTuple()}
            };

            foreach (var i in _infos)
            {
                i.Value.Item1.Text = $"{i.Key}:";
                i.Value.Item1.HorizontalOptions = LayoutOptions.StartAndExpand;

                InfoStackHeadings.Children.Add(i.Value.Item1);
                InfoStackValues.Children.Add(i.Value.Item2);
            }

            UpdateView();

            var info = UiUtils.Get.CoinInfo.GetInfos(_currencyId);
            if (UiUtils.Get.CoinInfo.InfosAvailable(_currencyId) && info == null && CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchCoinInfoFor(_currencyId);
            }

            PullToRefresh.RefreshCommand = new Command(Refresh);

            Messaging.Update.CoinInfos.Subscribe(this, () => UpdateView(true));
        }

        private void UpdateView(bool calledFromBackground = false)
        {
            var info = UiUtils.Get.CoinInfo.GetInfos(_currencyId);

            void UpdateUi()
            {
                _infos[I18N.Name].Item1.IsVisible = true;
                _infos[I18N.Name].Item2.IsVisible = true;
                _infos[I18N.Name].Item2.Text = _currencyId.FindName();

                _infos[I18N.CurrencyCode].Item1.IsVisible = true;
                _infos[I18N.CurrencyCode].Item2.IsVisible = true;
                _infos[I18N.CurrencyCode].Item2.Text = _currencyId.Code();

                _infos[I18N.CoinExplorer].Item1.IsVisible = info?.HasExplorer ?? false;
                _infos[I18N.CoinExplorer].Item2.IsVisible = info?.HasExplorer ?? false;
                _infos[I18N.CoinExplorer].Item2.Text = info?.Explorer;

                _referenceView.Items = (UiUtils.Get.CoinInfo.ReferenceValues(_currencyId), UiUtils.Get.CoinInfo.SortButtons);


                if (info != null)
                {
                    _infos[I18N.CoinAlgorithm].Item1.IsVisible = info.HasAlgorithm;
                    _infos[I18N.CoinAlgorithm].Item2.IsVisible = info.HasAlgorithm;
                    _infos[I18N.CoinAlgorithm].Item2.Text = info.Algorithm;

                    _infos[I18N.Type].Item1.IsVisible = info.HasType;
                    _infos[I18N.Type].Item2.IsVisible = info.HasType;
                    _infos[I18N.Type].Item2.Text = info.Type;

                    _infos[I18N.CoinHashrate].Item1.IsVisible = info.HasHashrate;
                    _infos[I18N.CoinHashrate].Item2.IsVisible = info.HasHashrate;
                    _infos[I18N.CoinHashrate].Item2.Text = info.Hashrate;

                    _infos[I18N.CoinDifficulty].Item1.IsVisible = info.HasDifficulty;
                    _infos[I18N.CoinDifficulty].Item2.IsVisible = info.HasDifficulty;
                    _infos[I18N.CoinDifficulty].Item2.Text = info.Difficulty;

                    _infos[I18N.CoinBlockReward].Item1.IsVisible = info.HasBlockreward;
                    _infos[I18N.CoinBlockReward].Item2.IsVisible = info.HasBlockreward;
                    _infos[I18N.CoinBlockReward].Item2.Text = info.Blockreward;

                    _infos[I18N.CoinBlockHeight].Item1.IsVisible = info.HasBlockheight;
                    _infos[I18N.CoinBlockHeight].Item2.IsVisible = info.HasBlockheight;
                    _infos[I18N.CoinBlockHeight].Item2.Text = info.Blockheight;

                    _infos[I18N.CoinBlocktime].Item1.IsVisible = info.HasBlocktime;
                    _infos[I18N.CoinBlocktime].Item2.IsVisible = info.HasBlocktime;
                    _infos[I18N.CoinBlocktime].Item2.Text = info.Blocktime;

                    _infos[I18N.CoinSupplyMax].Item1.IsVisible = info.HasMaxSupply;
                    _infos[I18N.CoinSupplyMax].Item2.IsVisible = info.HasMaxSupply;
                    _infos[I18N.CoinSupplyMax].Item2.Text = info.MaxSupply;

                    _infos[I18N.CoinSupply].Item1.IsVisible = info.HasSupply;
                    _infos[I18N.CoinSupply].Item2.IsVisible = info.HasSupply;
                    _infos[I18N.CoinSupply].Item2.Text = info.Supply;

                    _infos[I18N.CoinMarketCap].Item1.IsVisible = info.HasMarketCap;
                    _infos[I18N.CoinMarketCap].Item2.IsVisible = info.HasMarketCap;
                    _infos[I18N.CoinMarketCap].Item2.Text = info.MarketCap;
                }
                else
                {
                    _infos[I18N.CoinAlgorithm].Item1.IsVisible = false;
                    _infos[I18N.CoinAlgorithm].Item2.IsVisible = false;

                    _infos[I18N.Type].Item1.IsVisible = false;
                    _infos[I18N.Type].Item2.IsVisible = false;

                    _infos[I18N.CoinHashrate].Item1.IsVisible = false;
                    _infos[I18N.CoinHashrate].Item2.IsVisible = false;

                    _infos[I18N.CoinDifficulty].Item1.IsVisible = false;
                    _infos[I18N.CoinDifficulty].Item2.IsVisible = false;

                    _infos[I18N.CoinBlockReward].Item1.IsVisible = false;
                    _infos[I18N.CoinBlockReward].Item2.IsVisible = false;

                    _infos[I18N.CoinBlockHeight].Item1.IsVisible = false;
                    _infos[I18N.CoinBlockHeight].Item2.IsVisible = false;

                    _infos[I18N.CoinBlocktime].Item1.IsVisible = false;
                    _infos[I18N.CoinBlocktime].Item2.IsVisible = false;

                    _infos[I18N.CoinSupply].Item1.IsVisible = false;
                    _infos[I18N.CoinSupply].Item2.IsVisible = false;

                    _infos[I18N.CoinSupplyMax].Item1.IsVisible = false;
                    _infos[I18N.CoinSupplyMax].Item2.IsVisible = false;

                    _infos[I18N.CoinMarketCap].Item1.IsVisible = false;
                    _infos[I18N.CoinMarketCap].Item2.IsVisible = false;
                }
            }

            if (calledFromBackground)
            {
                Device.BeginInvokeOnMainThread(UpdateUi);
            }
            else
            {
                UpdateUi();
            }

            SetFooter();
        }

        private void SetFooter()
        {
            Device.BeginInvokeOnMainThread(() => Footer.Text = UiUtils.Get.CoinInfo.LastUpdate(_currencyId).LastUpdateString());
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchCoinInfoAndRatesFor(_currencyId);
                PullToRefresh.IsRefreshing = false;
            }
            else
            {
                PullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        private void ShowAccounts(object sender, EventArgs e)
        {
            var a = AccountStorage.AccountsWithCurrency(_currencyId);
            Navigation.PushAsync(a.Count == 1 ? new AccountView(a[0]) as Page : new AccountGroupView(_currencyId));
        }
    }
}
