using System.Collections.Generic;
using System.Linq;
using data.settings;
using data.storage;
using enums;
using message;
using MyCryptos.helpers;
using MyCryptos.models;
using MyCryptos.resources;
using tasks;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public class CoinsHeaderView : HeaderView
	{
		Currency currency;
		private List<string> InfoTexts;
		private static int currentInfoText = 1;

		public CoinsHeaderView(Currency currency)
		{
			this.currency = currency ?? ApplicationSettings.BaseCurrency;

			var recognizer = new TapGestureRecognizer();
			recognizer.Tapped += (sender, e) =>
			{
				currentInfoText = (currentInfoText + 1) % InfoTexts.Count;
				InfoText = InfoTexts[currentInfoText];
			};

			var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

			InfoTexts = new List<string> { string.Empty, string.Empty };

			Padding = new Thickness(0, 0, 0, 20);

			GestureRecognizers.Add(recognizer);
			addSubscriber();
			updateView();
		}

		void updateView()
		{
			var sum = moneySum;
			var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

			TitleText = (sum.Amount > 0) ? sum.ToString() : string.Format("0 {0}", sum.Currency.Code);
			InfoTexts[0] = PluralHelper.GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, amountDifferentCurrencies);
			InfoTexts[1] = string.Join(" | ", ApplicationSettings.ReferenceCurrencies.Where(c => !c.Equals(currency)).Select(c => MoneySum(c)?.ToString() ?? $"0 {c.Code}"));
			InfoText = InfoTexts[currentInfoText];
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
		}

		Money moneySum => MoneySum(currency);

		private static Money MoneySum(Currency currency)
		{
			var neededRates = new List<ExchangeRate>();

			var amount = AccountStorage.Instance.AllElements.Select(a =>
			{
				var neededRate = new ExchangeRate(a.Money.Currency, currency);
				var rate = ExchangeRateHelper.GetRate(neededRate);

				if (rate == null || !rate.Rate.HasValue)
				{
					neededRates.Add(neededRate);
				}

				return a.Money.Amount * (rate ?? neededRate).RateNotNull;
			}).Sum();

			AppTasks.Instance.StartMissingRatesTask(neededRates.Distinct());

			return new Money(amount, currency);
		}

		void addSubscriber()
		{
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => IsLoading = true);
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => IsLoading = false);
		}
	}
}
