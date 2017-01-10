using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinInfoView : ContentPage
	{
		private Currency _currency;
		private ReferenceCurrenciesView _referenceView;
		private List<Currency> _referenceCurrencies => ApplicationSettings.WatchedCurrencies.Concat(AccountStorage.UsedCurrencies).Where(c => !c.Equals(_currency)).ToList();
		private List<ExchangeRate> _rates => _referenceCurrencies.Select(c => new ExchangeRate(Currency.Btc, c)).ToList();

		public CoinInfoView(Currency currency)
		{
			InitializeComponent();

			_currency = currency;

			Title = _currency.Code;
			Header.TitleText = _currency.Name;

			_referenceView = new ReferenceCurrenciesView(new Money(1, _currency), true, _referenceCurrencies);
			Content.Children.Add(_referenceView);

			Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

			UpdateView(true);
		}

		private void UpdateView() => UpdateView(false);

		private void UpdateView(bool mainThread)
		{
			var rate = new ExchangeRate(Currency.Btc, _currency);
			rate = ExchangeRateHelper.GetRate(rate) ?? rate;
			var referenceMoney = new Money(rate.RateNotNull, Currency.Btc);

			Action uiAction = () =>
			{
				Header.InfoText = referenceMoney.ToString8Digits();
			};

			_referenceView.UpdateView();
			if (mainThread) uiAction();
			else Device.BeginInvokeOnMainThread(uiAction);
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
