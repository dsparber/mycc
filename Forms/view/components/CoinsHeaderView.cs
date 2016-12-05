using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.components
{
	public class CoinsHeaderView : HeaderView
	{

		private readonly Currency currency;
		private readonly bool useOnlyThisCurrency;
		private readonly List<string> infoTexts;

		private static int currentInfoText = 1;
		private static bool dataLoaded;
		private bool shouldBeLoading = true;

		public CoinsHeaderView(Currency currency = null, bool useOnlyThisCurrency = false) : this()
		{
			this.currency = currency ?? ApplicationSettings.BaseCurrency;
			this.useOnlyThisCurrency = useOnlyThisCurrency;

			UpdateView();
		}

		private CoinsHeaderView()
		{
			var recognizer = new TapGestureRecognizer();
			recognizer.Tapped += (sender, e) =>
			{
				currentInfoText = (currentInfoText + 1) % infoTexts.Count;
				InfoText = infoTexts[currentInfoText];
			};

			infoTexts = new List<string> { string.Empty, string.Empty };

			Padding = new Thickness(0, 0, 0, 20);

			GestureRecognizers.Add(recognizer);
			AddSubscriber();
		}

		private void UpdateView()
		{
			var sum = useOnlyThisCurrency ? coinSum : MoneySum;
			var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

			if (useOnlyThisCurrency)
			{
				infoTexts[0] = PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).ToList().Count);
			}
			else {
				infoTexts[0] = PluralHelper.GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, amountDifferentCurrencies);
			}
			infoTexts[1] = string.Join(" | ", ApplicationSettings.ReferenceCurrencies.Where(c => !c.Equals(currency)).Select(c => MoneySumOf(c)?.ToString() ?? $"0 {c.Code}"));


			Device.BeginInvokeOnMainThread(() =>
			{
				if (dataLoaded)
				{
					TitleText = (sum.Amount > 0) ? sum.ToString() : $"0 {sum.Currency.Code}";
					InfoText = infoTexts[currentInfoText];
					if (!shouldBeLoading)
					{
						IsLoading = false;
					}
				}
				else
				{
					shouldBeLoading = IsLoading;
					IsLoading = true;
				}
			});


		}

		private Money coinSum => new Money(AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), currency);

		private Money MoneySum => MoneySumOf(currency);

		private static Money MoneySumOf(Currency currency)
		{
			var neededRates = new List<ExchangeRate>();

			var amount = AccountStorage.Instance.AllElements.Select(a =>
			{
				var neededRate = new ExchangeRate(a.Money.Currency, currency);
				var rate = ExchangeRateHelper.GetRate(neededRate);

				if (rate?.Rate == null)
				{
					neededRates.Add(neededRate);
				}

				return a.Money.Amount * (rate ?? neededRate).RateNotNull;
			}).Sum();

			if (neededRates.Count == 0) return new Money(amount, currency);

			ApplicationTasks.FetchMissingRates(neededRates, Messaging.FetchMissingRates.SendStarted, Messaging.FetchMissingRates.SendFinished, ErrorOverlay.Display);

			return new Money(amount, currency);
		}

		private void AddSubscriber()
		{
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
			Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
			Messaging.Loading.SubscribeFinished(this, () => { dataLoaded = true; UpdateView(); });

			Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
			Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
		}
	}
}
