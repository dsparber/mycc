using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using constants;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.CoinInfo;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinInfoView : ContentPage
	{
		private Currency _currency;
		private ReferenceCurrenciesView _referenceView;
		private List<Currency> _referenceCurrencies => ApplicationSettings.AllReferenceCurrencies;
		private List<ExchangeRate> _rates => _referenceCurrencies.Select(c => new ExchangeRate(Currency.Btc, c)).ToList();

		private Dictionary<string, Tuple<Label, Label>> _infos;

		public CoinInfoView(Currency currency)
		{
			InitializeComponent();

			_currency = currency;

			Title = _currency.Code;
			Header.TitleText = _currency.Name;
			InfoHeading.Text = Device.OS == TargetPlatform.iOS ? I18N.Info.ToUpper() : I18N.Info;
			InfoHeading.TextColor = AppConstants.TableSectionColor;

			_referenceView = new ReferenceCurrenciesView(new Money(1, _currency), true, _referenceCurrencies);
			Content.Children.Add(_referenceView);

			//Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
			//Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

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
				{I18N.BlockHeight, getTuple()},
				{I18N.Blocktime, getTuple()},
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

		private void UpdateView()
		{
			var rate = new ExchangeRate(_currency, Currency.Btc);
			rate = ExchangeRateHelper.GetRate(rate) ?? rate;
			var referenceMoney = new Money(rate.RateNotNull, Currency.Btc);

			var explorer = CoinInfoStorage.Instance.GetExplorer(_currency).Select(e => e.Name);

			_infos[I18N.Name].Item1.IsVisible = true;
			_infos[I18N.Name].Item2.IsVisible = true;
			_infos[I18N.Name].Item2.Text = _currency.Name;

			_infos[I18N.Abbreviation].Item1.IsVisible = true;
			_infos[I18N.Abbreviation].Item2.IsVisible = true;
			_infos[I18N.Abbreviation].Item2.Text = _currency.Code;

			_infos[I18N.BlockExplorer].Item1.IsVisible = explorer.Count() > 0;
			_infos[I18N.BlockExplorer].Item2.IsVisible = explorer.Count() > 0;
			_infos[I18N.BlockExplorer].Item2.Text = string.Join(", ", explorer);

			Header.InfoText = referenceMoney.ToString8Digits();
			_referenceView.UpdateView();

			Task.Factory.StartNew(async () =>
			{
				var info = await CoinInfoStorage.Instance.GetInfo(_currency);

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
					_infos[I18N.Hashrate].Item2.Text = $"{info.Hashrate} {I18N.GHps}";

					_infos[I18N.Difficulty].Item1.IsVisible = info.Difficulty != null;
					_infos[I18N.Difficulty].Item2.IsVisible = info.Difficulty != null;
					_infos[I18N.Difficulty].Item2.Text = info.Difficulty.GetValueOrDefault().ToString();

					_infos[I18N.BlockHeight].Item1.IsVisible = info.Height != null;
					_infos[I18N.BlockHeight].Item2.IsVisible = info.Height != null;
					_infos[I18N.BlockHeight].Item2.Text = info.Height.GetValueOrDefault().ToString();

					_infos[I18N.Blocktime].Item1.IsVisible = info.Blocktime != null;
					_infos[I18N.Blocktime].Item2.IsVisible = info.Blocktime != null;
					_infos[I18N.Blocktime].Item2.Text = info.Blocktime.GetValueOrDefault().ToString();

					_infos[I18N.CoinSupply].Item1.IsVisible = info.CoinSupply != null;
					_infos[I18N.CoinSupply].Item2.IsVisible = info.CoinSupply != null;
					_infos[I18N.CoinSupply].Item2.Text = info.CoinSupply.GetValueOrDefault().ToString();

					_infos[I18N.MarketCap].Item1.IsVisible = info.CoinSupply != null && rate.Rate != null;
					_infos[I18N.MarketCap].Item2.IsVisible = info.CoinSupply != null && rate.Rate != null;
					_infos[I18N.MarketCap].Item2.Text = new Money(info.CoinSupply.GetValueOrDefault() * rate.RateNotNull, Currency.Btc).ToStringTwoDigits(ApplicationSettings.RoundMoney);
				}
				else {
					_infos[I18N.Algorithm].Item1.IsVisible = false;
					_infos[I18N.Algorithm].Item2.IsVisible = false;

					_infos[I18N.Type].Item1.IsVisible = false;
					_infos[I18N.Type].Item2.IsVisible = false;

					_infos[I18N.Hashrate].Item1.IsVisible = false;
					_infos[I18N.Hashrate].Item2.IsVisible = false;

					_infos[I18N.Difficulty].Item1.IsVisible = false;
					_infos[I18N.Difficulty].Item2.IsVisible = false;

					_infos[I18N.BlockHeight].Item1.IsVisible = false;
					_infos[I18N.BlockHeight].Item2.IsVisible = false;

					_infos[I18N.Blocktime].Item1.IsVisible = false;
					_infos[I18N.Blocktime].Item2.IsVisible = false;

					_infos[I18N.CoinSupply].Item1.IsVisible = false;
					_infos[I18N.CoinSupply].Item2.IsVisible = false;

					_infos[I18N.MarketCap].Item1.IsVisible = false;
					_infos[I18N.MarketCap].Item2.IsVisible = false;
				}
			});
		}

		private async void Refresh(object sender, EventArgs e)
		{
			await AppTaskHelper.FetchRates(_rates);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_referenceView.OnAppearing();
		}
	}
}
