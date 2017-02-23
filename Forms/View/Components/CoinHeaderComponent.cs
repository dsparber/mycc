using System;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class CoinHeaderComponent : HeaderView
    {
        private readonly Currency _currency;
        private FunctionalAccount _account;

        private readonly bool _useOnlyThisCurrency;

        public CoinHeaderComponent(FunctionalAccount account) : this()
        {
            _account = account;
            _currency = account.Money.Currency;
            _useOnlyThisCurrency = true;

            UpdateView();
        }

        public CoinHeaderComponent(Currency currency = null, bool useOnlyThisCurrency = false) : this()
        {
            _currency = currency ?? ApplicationSettings.BaseCurrency;
            _useOnlyThisCurrency = useOnlyThisCurrency;

            UpdateView();
        }

        private CoinHeaderComponent() : base(true)
        {
            AddSubscriber();
        }

        private void UpdateView()
        {
            string infoTextFallback;
            if (_account != null)
            {
                infoTextFallback = _account.Name;
            }
            else if (_useOnlyThisCurrency)
            {
                infoTextFallback = PluralHelper.GetTextAccounts(AccountStorage.AccountsWithCurrency(_currency).Count);
            }
            else
            {
                var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().Count();

                infoTextFallback = PluralHelper.GetTextCurrencies(amountDifferentCurrencies);
            }
            var infoText = string.Join(" / ", ApplicationSettings.MainCurrencies.Where(c => !c.Equals(_currency))
                                       .Select(c => ((_useOnlyThisCurrency ? CoinSumAs(c) : MoneySumOf(c)) ?? new Money(0, c))
                                       .ToStringTwoDigits(ApplicationSettings.RoundMoney)));

            infoText = string.IsNullOrWhiteSpace(infoText) ? infoTextFallback : infoText;

            if (_account != null)
            {
                _account = AccountStorage.Instance.AllElements.Find(a => a.Id.Equals(_account.Id));
            }


            Device.BeginInvokeOnMainThread(() =>
            {
                if (_useOnlyThisCurrency)
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

                InfoText = infoText;
            });
        }

        private Money Sum => _account != null ? _account.Money : (_useOnlyThisCurrency ? CoinSum : MoneySum) ?? new Money(0, _currency);

        private Money CoinSum => _account != null ? _account.Money : new Money(AccountStorage.EnabledAccounts.Where(a => _currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), _currency);
        private Money CoinSumAs(Currency c) => new Money(CoinSum.Amount * (ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.Rate ?? 0), c);

        private Money MoneySum => MoneySumOf(_currency);

        private static Money MoneySumOf(Currency currency)
        {
            var amount = AccountStorage.EnabledAccounts.Sum(a =>
            {
                var rate = new ExchangeRate(a.Money.Currency, currency);
                rate = ExchangeRateHelper.GetRate(rate) ?? rate;

                return a.Money.Amount * rate.Rate ?? 0;
            });

            return new Money(amount, currency);
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.RoundNumbers.SubscribeValueChanged(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingRates.SubscribeFinished(this, UpdateView);
        }
    }
}
