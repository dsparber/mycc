using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using Xamarin.Forms;
using MyCC.Forms.Messages;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using Plugin.Connectivity;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinInfoView
    {
        private readonly Currency _currency;
        private readonly ReferenceCurrenciesView _referenceView;


        private readonly Dictionary<string, Tuple<Label, Label>> _infos;

        public CoinInfoView(Currency currency, bool enableAccountButton = false)
        {
            InitializeComponent();

            _currency = currency;

            if (!enableAccountButton || !AccountStorage.AccountsWithCurrency(_currency).Any())
            {
                ToolbarItems.Remove(AccountsButton);
            }

            Title = _currency.Code;
            var header = new CoinInfoHeaderComponent(_currency);
            ChangingStack.Children.Insert(0, header);
            InfoHeading.Text = Device.RuntimePlatform.Equals(Device.iOS) ? I18N.Info.ToUpper() : I18N.Info;
            InfoHeading.TextColor = AppConstants.TableSectionColor;

            _referenceView = new ReferenceCurrenciesView(new Money(1, _currency));
            ContentView.Children.Add(_referenceView);

            Func<Label> getLabel = () => new Label { IsVisible = false, FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.TableSectionColor, LineBreakMode = LineBreakMode.TailTruncation };
            Func<Tuple<Label, Label>> getTuple = () => Tuple.Create(getLabel(), getLabel());

            _infos = new Dictionary<string, Tuple<Label, Label>> {
                {I18N.CurrencyCode, getTuple()},
                {I18N.Name, getTuple()},

                {I18N.CoinExplorer, getTuple()},
                {I18N.CoinAlgorithm, getTuple()},
                {I18N.Type, getTuple()},

                {I18N.CoinHashrate, getTuple()},
                {I18N.CoinDifficulty, getTuple()},
                {I18N.CoinBlockReward, getTuple()},
                {I18N.CoinBlockHeight, getTuple()},
                {I18N.CoinBlocktime, getTuple()},
                {I18N.CoinSupplyMax, getTuple()},
                {I18N.CoinSupply, getTuple()},
                {I18N.CoinMarketCap, getTuple()}
            };

            foreach (var i in _infos)
            {
                i.Value.Item1.Text = $"{i.Key}:";
                i.Value.Item1.HorizontalOptions = LayoutOptions.StartAndExpand;

                InfoStackHeadings.Children.Add(i.Value.Item1);
                InfoStackValues.Children.Add(i.Value.Item2);
            }

            UpdateView();

            var explorer = CoinInfoStorage.Instance.GetExplorer(_currency).Select(e => e.Name).ToList();
            var info = CoinInfoStorage.Instance.Get(_currency);
            if (info == null && explorer.Any() && CrossConnectivity.Current.IsConnected)
            {
                Task.Run(() => AppTaskHelper.FetchCoinInfo(_currency));
            }

            PullToRefresh.RefreshCommand = new Command(Refresh);

            Messaging.Progress.SubscribeToComplete(this, () => UpdateView(true));
        }

        private void UpdateView(bool calledFromBackground = false)
        {
            var rate = new ExchangeRate(_currency, CurrencyConstants.Btc);
            rate = ExchangeRateHelper.GetRate(rate) ?? rate;

            var explorer = CoinInfoStorage.Instance.GetExplorer(_currency).Select(e => e.Name).ToList();
            var info = CoinInfoStorage.Instance.Get(_currency);

            Action updateUi = () =>
            {
                _infos[I18N.Name].Item1.IsVisible = true;
                _infos[I18N.Name].Item2.IsVisible = true;
                _infos[I18N.Name].Item2.Text = _currency.Name;

                _infos[I18N.CurrencyCode].Item1.IsVisible = true;
                _infos[I18N.CurrencyCode].Item2.IsVisible = true;
                _infos[I18N.CurrencyCode].Item2.Text = _currency.Code;

                _infos[I18N.CoinExplorer].Item1.IsVisible = explorer.Any();
                _infos[I18N.CoinExplorer].Item2.IsVisible = explorer.Any();
                _infos[I18N.CoinExplorer].Item2.Text = string.Join(", ", explorer);

                _referenceView.UpdateView();


                if (info != null)
                {
                    _infos[I18N.CoinAlgorithm].Item1.IsVisible = info.Algorithm != null;
                    _infos[I18N.CoinAlgorithm].Item2.IsVisible = info.Algorithm != null;
                    _infos[I18N.CoinAlgorithm].Item2.Text = info.Algorithm;

                    _infos[I18N.Type].Item1.IsVisible = info.IsProofOfWork != null || info.IsProofOfStake != null;
                    _infos[I18N.Type].Item2.IsVisible = info.IsProofOfWork != null || info.IsProofOfStake != null;
                    _infos[I18N.Type].Item2.Text = info.IsProofOfWork.GetValueOrDefault() && info.IsProofOfStake.GetValueOrDefault() ? $"{I18N.CoinProofOfWork}, {I18N.CoinProofOfStake}" : info.IsProofOfWork.GetValueOrDefault() ? I18N.CoinProofOfWork : info.IsProofOfStake.GetValueOrDefault() ? I18N.CoinProofOfStake : string.Empty;

                    _infos[I18N.CoinHashrate].Item1.IsVisible = info.Hashrate != null;
                    _infos[I18N.CoinHashrate].Item2.IsVisible = info.Hashrate != null;
                    _infos[I18N.CoinHashrate].Item2.Text = $"{info.Hashrate:#,0.########} {I18N.GHps}";

                    _infos[I18N.CoinDifficulty].Item1.IsVisible = info.Difficulty != null;
                    _infos[I18N.CoinDifficulty].Item2.IsVisible = info.Difficulty != null;
                    _infos[I18N.CoinDifficulty].Item2.Text = $"{info.Difficulty.GetValueOrDefault():#,0.########}";

                    _infos[I18N.CoinBlockReward].Item1.IsVisible = info.Blockreward != null;
                    _infos[I18N.CoinBlockReward].Item2.IsVisible = info.Blockreward != null;
                    _infos[I18N.CoinBlockReward].Item2.Text = new Money(info.Blockreward.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.CoinBlockHeight].Item1.IsVisible = info.BlockHeight != null;
                    _infos[I18N.CoinBlockHeight].Item2.IsVisible = info.BlockHeight != null;
                    _infos[I18N.CoinBlockHeight].Item2.Text = $"{info.BlockHeight.GetValueOrDefault():#,0}";

                    _infos[I18N.CoinBlocktime].Item1.IsVisible = info.Blocktime != null;
                    _infos[I18N.CoinBlocktime].Item2.IsVisible = info.Blocktime != null;
                    _infos[I18N.CoinBlocktime].Item2.Text = $"{ info.Blocktime.GetValueOrDefault():#,0.##} {I18N.UnitSecond}";

                    _infos[I18N.CoinSupplyMax].Item1.IsVisible = info.MaxCoinSupply != null;
                    _infos[I18N.CoinSupplyMax].Item2.IsVisible = info.MaxCoinSupply != null;
                    _infos[I18N.CoinSupplyMax].Item2.Text = new Money(info.MaxCoinSupply.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.CoinSupply].Item1.IsVisible = info.CoinSupply != null;
                    _infos[I18N.CoinSupply].Item2.IsVisible = info.CoinSupply != null;
                    _infos[I18N.CoinSupply].Item2.Text = new Money(info.CoinSupply.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.CoinMarketCap].Item1.IsVisible = info.CoinSupply != null && rate.Rate != null;
                    _infos[I18N.CoinMarketCap].Item2.IsVisible = info.CoinSupply != null && rate.Rate != null;
                    _infos[I18N.CoinMarketCap].Item2.Text = new Money(info.CoinSupply.GetValueOrDefault() * rate.Rate ?? 0, CurrencyConstants.Btc).ToStringTwoDigits(ApplicationSettings.RoundMoney);
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
            };

            if (calledFromBackground)
            {
                Device.BeginInvokeOnMainThread(updateUi);
            }
            else
            {
                updateUi();
            }

            SetFooter();
        }

        private void SetFooter()
        {
            var ratesTime = ApplicationSettings.AllReferenceCurrencies
                                    .Select(e => new ExchangeRate(_currency, e))
                                    .SelectMany(ExchangeRateHelper.GetNeededRates)
                                    .Distinct()
                                    .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty().Min();

            var infoTime = CoinInfoStorage.Instance.Get(_currency)?.LastUpdate ?? DateTime.Now;

            var text = (ratesTime < infoTime ? ratesTime : infoTime).LastUpdateString();

            Device.BeginInvokeOnMainThread(() => Footer.Text = text);
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await AppTaskHelper.FetchCoinDetails(_currency);
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
            var a = AccountStorage.AccountsWithCurrency(_currency);
            Navigation.PushAsync(a.Count == 1 ? new AccountView(a[0]) as Page : new AccountGroupView(_currency));
        }
    }
}
