using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.components
{
	public class CoinHeaderComponent : HeaderView
	{

		private readonly Currency currency;
		private readonly decimal? amount;
		private readonly FunctionalAccount account;
		private readonly bool useOnlyThisCurrency;
		private readonly List<string> infoTexts;

		private static int currentInfoText = 1;

		public CoinHeaderComponent(FunctionalAccount account) : this()
		{
			this.account = account;
			currency = account.Money.Currency;
			useOnlyThisCurrency = true;

			UpdateView();
		}

		public CoinHeaderComponent(Currency currency = null, bool useOnlyThisCurrency = false, decimal? amount = null) : this()
		{
			this.currency = currency ?? ApplicationSettings.BaseCurrency;
			this.amount = amount;
			this.useOnlyThisCurrency = useOnlyThisCurrency || amount != null;

			UpdateView();
		}

		private CoinHeaderComponent() : base(true)
		{
			var recognizer = new TapGestureRecognizer();
			recognizer.Tapped += (sender, e) =>
			{
				currentInfoText = (currentInfoText + 1) % infoTexts.Count;
				InfoText = infoTexts[currentInfoText];
			};

			infoTexts = new List<string> { string.Empty, string.Empty };

			GestureRecognizers.Add(recognizer);
			AddSubscriber();
		}

		private void UpdateView(bool? isLoading = null)
		{
			var sum = amount != null ? new Money(amount.Value, currency) : account != null ? account.Money : (useOnlyThisCurrency ? CoinSum : MoneySum) ?? new Money(0, currency);
			var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

			if (amount != null)
			{
				infoTexts[0] = string.Empty;
			}
			else if (account != null)
			{
				infoTexts[0] = PluralHelper.GetTextAccounts(1);
			}
			else if (useOnlyThisCurrency)
			{
				infoTexts[0] = PluralHelper.GetTextAccounts(AccountStorage.AccountsWithCurrency(currency).Count);
			}
			else
			{
				infoTexts[0] = PluralHelper.GetTextCoins(amountDifferentCurrencies);
			}
			infoTexts[1] = useOnlyThisCurrency ? currency?.Name : string.Join(" / ", ApplicationSettings.MainReferenceCurrencies
									   .Where(c => !c.Equals(currency))
									   .Select(c => (amount != null ? new Money(ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.RateNotNull ?? 0, c)
													 : (useOnlyThisCurrency ? CoinSumAs(c)
														: MoneySumOf(c)) ?? new Money(0, c))
											   .ToStringTwoDigits(ApplicationSettings.RoundMoney)));


			Device.BeginInvokeOnMainThread(() =>
			{
				TitleText = useOnlyThisCurrency ? sum.ToString(false) : sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
				InfoText = infoTexts[currentInfoText];

				if (isLoading.HasValue)
				{
					IsLoading = isLoading.Value;
				}
			});


		}

		private Money CoinSum => account != null ? account.Money : new Money(AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), currency);
		private Money CoinSumAs(Currency c) => new Money(CoinSum.Amount * (ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.RateNotNull ?? 0), c);

		private Money MoneySum => MoneySumOf(currency);

		private static Money MoneySumOf(Currency currency)
		{
			var amount = AccountStorage.Instance.AllElements.Sum(a =>
			{
				var neededRate = new ExchangeRate(a.Money.Currency, currency);
				var rate = ExchangeRateHelper.GetRate(neededRate);

				if (rate != null && rate.Rate == null)
				{
				}

				return a.Money.Amount * (rate ?? neededRate).RateNotNull;
			});

			return new Money(amount, currency);
		}

		private void AddSubscriber()
		{
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => UpdateView());
			Messaging.RoundNumbers.SubscribeValueChanged(this, () => UpdateView());
			Messaging.UpdatingAccounts.SubscribeFinished(this, () => UpdateView());
			Messaging.Loading.SubscribeFinished(this, () => UpdateView());
		}
	}
}
