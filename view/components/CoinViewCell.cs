using System;
using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using models;
using MyCryptos.resources;
using Xamarin.Forms;
using view;

namespace MyCryptos.view.components
{
	public class CoinViewCell : CustomViewCell
	{
		readonly INavigation Navigation;

		IEnumerable<Tuple<Account, AccountRepository>> accounts;
		ExchangeRate exchangeRate;

		public ExchangeRate ExchangeRate
		{
			get { return exchangeRate; }
			set { exchangeRate = value; Detail = MoneyReference != null ? MoneyReference.ToString() : string.Empty; setTapRecognizer(); }
		}
		public IEnumerable<Tuple<Account, AccountRepository>> Accounts
		{
			get { if (accounts == null) { accounts = new List<Tuple<Account, AccountRepository>>(); } return accounts; }
			set { accounts = value; Text = MoneySum != null ? MoneySum.ToString() : string.Empty; setTapRecognizer(); }
		}

		public AccountRepository repository(Account account)
		{
			return Accounts.ToList().Find(t => t.Item1 == account).Item2;
		}

		public Currency Currency
		{
			get { return Accounts.ToList().Count > 0 ? Accounts.First().Item1.Money.Currency : null; }
		}

		public Money MoneySum
		{
			get
			{
				if (Accounts.ToList().Count > 0)
				{
					return new Money(Accounts.Sum(a => a.Item1.Money.Amount), Accounts.First().Item1.Money.Currency);
				}
				return null;
			}
		}

		public Money MoneyReference
		{
			get
			{
				if (exchangeRate != null && exchangeRate.Rate.HasValue && MoneySum != null)
				{
					return new Money(MoneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				}
				return null;
			}
		}

		public CoinViewCell(INavigation navigation)
		{
			Navigation = navigation;
			Image = "more.png";
			Detail = InternationalisationResources.NoExchangeRateFound;
			setTapRecognizer();
		}

		void setTapRecognizer()
		{
			var gestureRecognizer = new TapGestureRecognizer();
			gestureRecognizer.Tapped += (sender, e) =>
			{
				if (accounts.ToList().Count > 1)
				{
					Navigation.PushAsync(new CoinDetailView(accounts, exchangeRate));
				}
				else {
					var element = accounts.ToList()[0];
					Navigation.PushAsync(new AccountDetailView(element.Item1, element.Item2));
				}
			};
			if (View != null)
			{
				View.GestureRecognizers.Clear();
				View.GestureRecognizers.Add(gestureRecognizer);
			}
		}

		public override decimal Units { get { return MoneySum.Amount; } }
		public override string Name { get { return MoneySum.Currency.Code; } }
		public override decimal Value { get { return MoneySum.Amount * (ExchangeRate != null ? ExchangeRate.RateNotNull : 0); } }
	}
}


