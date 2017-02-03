﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.view.components
{
	public class CoinHeaderComponent : HeaderView
	{
		private readonly Currency _currency;
		private FunctionalAccount _account;

		private readonly bool _useOneBitcoinAsReference;
		private readonly bool _useOnlyThisCurrency;

		private readonly List<string> _infoTexts;
		private static int _currentInfoText = 1;

		public CoinHeaderComponent(FunctionalAccount account) : this()
		{
			_account = account;
			_currency = account.Money.Currency;
			_useOnlyThisCurrency = true;

			UpdateView();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			SetInfoText();
		}

		public CoinHeaderComponent(Currency currency = null, bool useOnlyThisCurrency = false, bool useOneBitcoinAsReference = false) : this()
		{
			_currency = currency ?? ApplicationSettings.BaseCurrency;
			_useOneBitcoinAsReference = useOneBitcoinAsReference;
			_useOnlyThisCurrency = useOnlyThisCurrency;

			UpdateView();
		}

		private CoinHeaderComponent() : base(true)
		{
			var recognizer = new TapGestureRecognizer();
			recognizer.Tapped += (sender, e) =>
			{
				_currentInfoText = (_currentInfoText + 1) % _infoTexts.Count;
				SetInfoText();
			};

			_infoTexts = new List<string> { string.Empty, string.Empty };

			GestureRecognizers.Add(recognizer);
			AddSubscriber();
		}

		private void UpdateView(bool? isLoading = null)
		{
			var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

			if (_useOneBitcoinAsReference)
			{
				_infoTexts[0] = _currency.Name;
			}
			else if (_account != null)
			{
				_infoTexts[0] = _account.Name;
			}
			else if (_useOnlyThisCurrency)
			{
				_infoTexts[0] = PluralHelper.GetTextAccounts(AccountStorage.AccountsWithCurrency(_currency).Count);
			}
			else
			{
				_infoTexts[0] = PluralHelper.GetTextCurrencies(amountDifferentCurrencies);
			}
			_infoTexts[1] = (_useOneBitcoinAsReference) ? _currency?.Name : string.Join(" / ", ApplicationSettings.MainCurrencies
									   .Where(c => !c.Equals(_currency))
									   .Select(c => (_useOneBitcoinAsReference ? new Money(ExchangeRateHelper.GetRate(Currency.Btc, c)?.Rate ?? 0, c)
													 : (_useOnlyThisCurrency ? CoinSumAs(c)
														: MoneySumOf(c)) ?? new Money(0, c))
											   .ToStringTwoDigits(ApplicationSettings.RoundMoney)));

			if (_account != null)
			{
				_account = AccountStorage.Instance.AllElements.Find(a => a.Id.Equals(_account.Id));
			}


			Device.BeginInvokeOnMainThread(() =>
			{
				if (_useOnlyThisCurrency && !_useOneBitcoinAsReference)
				{
					var s = Sum.ToString(false);
					var beforeDecimal = new Money(Math.Truncate(Sum.Amount), Sum.Currency).ToString(false);
					var decimals = s.Remove(0, beforeDecimal.Length);
					var i1 = decimals.IndexOf(".", StringComparison.CurrentCulture);
					var i2 = decimals.IndexOf(",", StringComparison.CurrentCulture);
					var i = i1 > i2 ? i1 : i2;
					i = i == -1 ? s.Length : i;
					i += 4 + beforeDecimal.Length;
					i = i > s.Length ? s.Length : i;
					TitleText = s.Substring(0, i);
					TitleTextSmall = s.Substring(i);
				}
				else
				{
					TitleText = Sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
				}

				SetInfoText();

				if (isLoading.HasValue)
				{
					IsLoading = isLoading.Value;
				}
			});
		}

		private Money Sum => _useOneBitcoinAsReference ? new Money(ExchangeRateHelper.GetRate(Currency.Btc, _currency).Rate ?? 0, _currency) : _account != null ? _account.Money : (_useOnlyThisCurrency ? CoinSum : MoneySum) ?? new Money(0, _currency);

		private Money CoinSum => _account != null ? _account.Money : new Money(AccountStorage.Instance.AllElements.Where(a => _currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), _currency);
		private Money CoinSumAs(Currency c) => new Money(CoinSum.Amount * (ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.Rate ?? 0), c);

		private Money MoneySum => MoneySumOf(_currency);

		private static Money MoneySumOf(Currency currency)
		{
			var amount = AccountStorage.Instance.AllElements.Sum(a =>
			{
				var rate = new ExchangeRate(a.Money.Currency, currency);
				rate = ExchangeRateHelper.GetRate(rate) ?? rate;

				return a.Money.Amount * rate.Rate ?? 0;
			});

			return new Money(amount, currency);
		}

		private void AddSubscriber()
		{
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => UpdateView());
			Messaging.RoundNumbers.SubscribeValueChanged(this, () => UpdateView());
			Messaging.Loading.SubscribeFinished(this, () => UpdateView());

			Messaging.FetchMissingRates.SubscribeFinished(this, () => UpdateView());
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, () => UpdateView());
			Messaging.UpdatingAccounts.SubscribeFinished(this, () => UpdateView());
		}

		private void SetInfoText()
		{
			if (_infoTexts == null || _infoTexts.Count < _currentInfoText) return;

			var text = _infoTexts[_currentInfoText];
			if (string.IsNullOrEmpty(text?.Trim()))
			{
				text = _infoTexts[(_currentInfoText + 1) % _infoTexts.Count];
			}
			InfoText = text;
		}
	}
}
