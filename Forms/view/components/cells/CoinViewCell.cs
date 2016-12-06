using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Forms.view.pages;

namespace MyCryptos.view.components
{
    public class CoinViewCell : CustomViewCell
    {
        private readonly INavigation navigation;

        private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> accounts;
        private ExchangeRate exchangeRate;

        public ExchangeRate ExchangeRate
        {
            private get { return exchangeRate; }
            set { exchangeRate = value; Detail = MoneyReference != null ? MoneyReference.ToStringTwoDigits() : I18N.NoExchangeRateFound; SetTapRecognizer(); }
        }
        public IEnumerable<Tuple<FunctionalAccount, AccountRepository>> Accounts
        {
            private get { return accounts ?? (accounts = new List<Tuple<FunctionalAccount, AccountRepository>>()); }
            set { accounts = value; Text = MoneySum != null ? MoneySum.ToString() : string.Empty; SetTapRecognizer(); }
        }

        public Currency Currency => Accounts.ToList().Count > 0 ? Accounts.First().Item1.Money.Currency : null;

        private Money MoneySum => Accounts.ToList().Count > 0 ? new Money(Accounts.Sum(a => a.Item1.Money.Amount), Accounts.First().Item1.Money.Currency) : null;

        private Money MoneyReference => (exchangeRate?.Rate != null && MoneySum != null) ? new Money(MoneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency) : null;

        public CoinViewCell(INavigation navigation)
        {
            this.navigation = navigation;
            Image = "more.png";
            Detail = I18N.NoExchangeRateFound;
            SetTapRecognizer();
        }

        private void SetTapRecognizer()
        {
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) =>
            {
                navigation.PushAsync(new CoinDetailView(Currency));
            };
            if (View != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    View.GestureRecognizers.Clear();
                    View.GestureRecognizers.Add(gestureRecognizer);
                });
            }
        }

        public override decimal Units => MoneySum.Amount;
        public override string Name => MoneySum.Currency.Code;
        public override decimal Value => MoneySum.Amount * (ExchangeRate?.RateNotNull ?? 0);
    }
}


