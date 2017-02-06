using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinInfoView
    {
        private readonly Currency _currency;
        private readonly ReferenceCurrenciesView _referenceView;

        private readonly Dictionary<string, Tuple<Label, Label>> _infos;

        public CoinInfoView(Currency currency)
        {
            InitializeComponent();

            _currency = currency;

            Title = _currency.Code;
            var header = new CoinInfoHeaderComponent(_currency);
            ChangingStack.Children.Insert(0, header);
            InfoHeading.Text = Device.OS == TargetPlatform.iOS ? I18N.Info.ToUpper() : I18N.Info;
            InfoHeading.TextColor = AppConstants.TableSectionColor;

            _referenceView = new ReferenceCurrenciesView(new Money(1, _currency), true);
            ContentView.Children.Add(_referenceView);

            Func<Label> getLabel = () => new Label { IsVisible = false, FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.TableSectionColor, LineBreakMode = LineBreakMode.TailTruncation };
            Func<Tuple<Label, Label>> getTuple = () => Tuple.Create(getLabel(), getLabel());

            _infos = new Dictionary<string, Tuple<Label, Label>> {
                {I18N.Name, getTuple()},
                {I18N.Abbreviation, getTuple()},

                {I18N.BlockExplorer, getTuple()},
                {I18N.Algorithm, getTuple()},
                {I18N.Type, getTuple()},

                {I18N.Hashrate, getTuple()},
                {I18N.Difficulty, getTuple()},
                {I18N.BlockReward, getTuple()},
                {I18N.BlockHeight, getTuple()},
                {I18N.Blocktime, getTuple()},
                {I18N.MaxCoinSupply, getTuple()},
                {I18N.CoinSupply, getTuple()},
                {I18N.MarketCap, getTuple()}
            };

            foreach (var i in _infos)
            {
                i.Value.Item1.Text = $"{i.Key}:";
                i.Value.Item1.HorizontalOptions = LayoutOptions.StartAndExpand;

                InfoStackHeadings.Children.Add(i.Value.Item1);
                InfoStackValues.Children.Add(i.Value.Item2);
            }

            UpdateView();
        }

        private void UpdateView(bool calledFromBackground = false)
        {
            var rate = new ExchangeRate(_currency, Currency.Btc);
            rate = ExchangeRateHelper.GetRate(rate) ?? rate;

            var explorer = CoinInfoStorage.Instance.GetExplorer(_currency).Select(e => e.Name).ToList();
            var info = CoinInfoStorage.Instance.Get(_currency);

            if (info == null && explorer.Any())
            {
                Task.Run(() => AppTaskHelper.FetchCoinInfo(_currency));
            }

            Action updateUi = () =>
            {
                _infos[I18N.Name].Item1.IsVisible = true;
                _infos[I18N.Name].Item2.IsVisible = true;
                _infos[I18N.Name].Item2.Text = _currency.Name;

                _infos[I18N.Abbreviation].Item1.IsVisible = true;
                _infos[I18N.Abbreviation].Item2.IsVisible = true;
                _infos[I18N.Abbreviation].Item2.Text = _currency.Code;

                _infos[I18N.BlockExplorer].Item1.IsVisible = explorer.Any();
                _infos[I18N.BlockExplorer].Item2.IsVisible = explorer.Any();
                _infos[I18N.BlockExplorer].Item2.Text = string.Join(", ", explorer);

                _referenceView.UpdateView();


                if (info != null)
                {
                    _infos[I18N.Algorithm].Item1.IsVisible = info.Algorithm != null;
                    _infos[I18N.Algorithm].Item2.IsVisible = info.Algorithm != null;
                    _infos[I18N.Algorithm].Item2.Text = info.Algorithm;

                    _infos[I18N.Type].Item1.IsVisible = info.IsProofOfWork != null || info.IsProofOfStake != null;
                    _infos[I18N.Type].Item2.IsVisible = info.IsProofOfWork != null || info.IsProofOfStake != null;
                    _infos[I18N.Type].Item2.Text = (info.IsProofOfWork.GetValueOrDefault() && info.IsProofOfStake.GetValueOrDefault()) ? I18N.ProofOfWorkAndState : (info.IsProofOfWork.GetValueOrDefault()) ? I18N.ProofOfWork : info.IsProofOfStake.GetValueOrDefault() ? I18N.ProofOfStake : string.Empty;

                    _infos[I18N.Hashrate].Item1.IsVisible = info.Hashrate != null;
                    _infos[I18N.Hashrate].Item2.IsVisible = info.Hashrate != null;
                    _infos[I18N.Hashrate].Item2.Text = $"{info.Hashrate:#,0.########} {I18N.GHps}";

                    _infos[I18N.Difficulty].Item1.IsVisible = info.Difficulty != null;
                    _infos[I18N.Difficulty].Item2.IsVisible = info.Difficulty != null;
                    _infos[I18N.Difficulty].Item2.Text = $"{info.Difficulty.GetValueOrDefault():#,0.########}";

                    _infos[I18N.BlockReward].Item1.IsVisible = info.Blockreward != null;
                    _infos[I18N.BlockReward].Item2.IsVisible = info.Blockreward != null;
                    _infos[I18N.BlockReward].Item2.Text = new Money(info.Blockreward.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.BlockHeight].Item1.IsVisible = info.BlockHeight != null;
                    _infos[I18N.BlockHeight].Item2.IsVisible = info.BlockHeight != null;
                    _infos[I18N.BlockHeight].Item2.Text = $"{info.BlockHeight.GetValueOrDefault():#,0}";

                    _infos[I18N.Blocktime].Item1.IsVisible = info.Blocktime != null;
                    _infos[I18N.Blocktime].Item2.IsVisible = info.Blocktime != null;
                    _infos[I18N.Blocktime].Item2.Text = $"{ info.Blocktime.GetValueOrDefault():#,0.##}{I18N.UnitSecond}";

                    _infos[I18N.MaxCoinSupply].Item1.IsVisible = info.MaxCoinSupply != null;
                    _infos[I18N.MaxCoinSupply].Item2.IsVisible = info.MaxCoinSupply != null;
                    _infos[I18N.MaxCoinSupply].Item2.Text = new Money(info.MaxCoinSupply.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.CoinSupply].Item1.IsVisible = info.CoinSupply != null;
                    _infos[I18N.CoinSupply].Item2.IsVisible = info.CoinSupply != null;
                    _infos[I18N.CoinSupply].Item2.Text = new Money(info.CoinSupply.GetValueOrDefault(), _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);

                    _infos[I18N.MarketCap].Item1.IsVisible = info.CoinSupply != null && rate.Rate != null;
                    _infos[I18N.MarketCap].Item2.IsVisible = info.CoinSupply != null && rate.Rate != null;
                    _infos[I18N.MarketCap].Item2.Text = new Money(info.CoinSupply.GetValueOrDefault() * rate.Rate ?? 0, Currency.Btc).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                }
                else
                {
                    _infos[I18N.Algorithm].Item1.IsVisible = false;
                    _infos[I18N.Algorithm].Item2.IsVisible = false;

                    _infos[I18N.Type].Item1.IsVisible = false;
                    _infos[I18N.Type].Item2.IsVisible = false;

                    _infos[I18N.Hashrate].Item1.IsVisible = false;
                    _infos[I18N.Hashrate].Item2.IsVisible = false;

                    _infos[I18N.Difficulty].Item1.IsVisible = false;
                    _infos[I18N.Difficulty].Item2.IsVisible = false;

                    _infos[I18N.BlockReward].Item1.IsVisible = false;
                    _infos[I18N.BlockReward].Item2.IsVisible = false;

                    _infos[I18N.BlockHeight].Item1.IsVisible = false;
                    _infos[I18N.BlockHeight].Item2.IsVisible = false;

                    _infos[I18N.Blocktime].Item1.IsVisible = false;
                    _infos[I18N.Blocktime].Item2.IsVisible = false;

                    _infos[I18N.CoinSupply].Item1.IsVisible = false;
                    _infos[I18N.CoinSupply].Item2.IsVisible = false;

                    _infos[I18N.MaxCoinSupply].Item1.IsVisible = false;
                    _infos[I18N.MaxCoinSupply].Item2.IsVisible = false;

                    _infos[I18N.MarketCap].Item1.IsVisible = false;
                    _infos[I18N.MarketCap].Item2.IsVisible = false;
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
        }

        private async void Refresh(object sender, EventArgs e)
        {
            RefreshItem.Clicked -= Refresh;
            await AppTaskHelper.FetchCoinDetails(_currency);
            RefreshItem.Clicked += Refresh;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _referenceView.OnAppearing();
        }
    }
}
