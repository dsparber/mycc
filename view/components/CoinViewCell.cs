using System;
using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view.components
{
	public class CoinViewCell : ViewCell
	{
		readonly Label SumMoneyLabel;
		readonly Label ReferenceValueLabel;

		readonly INavigation Navigation;

		IEnumerable<Tuple<Account, AccountRepository>> accounts;
		ExchangeRate exchangeRate;

		public ExchangeRate ExchangeRate
		{
			get
			{
				return exchangeRate;
			}
			set
			{
				exchangeRate = value;
				ReferenceValueLabel.Text = moneyReference != null ? moneyReference.ToString() : string.Empty;
				setTapRecognizer();
			}
		}
		public IEnumerable<Tuple<Account, AccountRepository>> Accounts
		{
			get
			{
				if (accounts == null)
				{
					accounts = new List<Tuple<Account, AccountRepository>>();
				}
				return accounts;
			}
			set
			{
				accounts = value;
				SumMoneyLabel.Text = moneySum != null ? moneySum.ToString() : string.Empty;
				setTapRecognizer();
			}
		}

		public AccountRepository repository(Account account)
		{
			return Accounts.ToList().Find(t => t.Item1 == account).Item2;
		}

		public Currency Currency
		{
			get
			{
				return Accounts.ToList().Count > 0 ? Accounts.First().Item1.Money.Currency : null;
			}
		}

		Money moneySum
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

		public Money MoneySum { get { return moneySum; } }

		Money moneyReference
		{
			get
			{
				if (exchangeRate != null && exchangeRate.Rate.HasValue && moneySum != null)
				{
					return new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				}
				return null;
			}
		}

		public Money MoneyReference { get { return moneyReference; } }

		public bool IsLoading
		{
			set
			{
				if (value)
				{
					ReferenceValueLabel.Text = InternationalisationResources.RefreshingDots;
				}
				else {
					ReferenceValueLabel.Text = moneyReference != null ? moneyReference.ToString() : InternationalisationResources.NoExchangeRateFound;
				}
			}
		}

		public CoinViewCell(INavigation navigation)
		{
			Navigation = navigation;

			SumMoneyLabel = new Label();
			ReferenceValueLabel = new Label();

			ReferenceValueLabel.TextColor = Color.Gray;
			ReferenceValueLabel.FontSize = SumMoneyLabel.FontSize * 0.75;

			var stack = new StackLayout();
			stack.Spacing = 0;
			stack.Children.Add(SumMoneyLabel);
			stack.Children.Add(ReferenceValueLabel);

			var icon = new Image { HeightRequest = 20, Source = ImageSource.FromFile("more.png") };
			icon.HorizontalOptions = LayoutOptions.EndAndExpand;

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(stack);
			horizontalStack.Children.Add(icon);
			horizontalStack.VerticalOptions = LayoutOptions.CenterAndExpand;

			var contentView = new ContentView();
			contentView.Margin = new Thickness(15, 0);
			contentView.Content = horizontalStack;

			View = contentView;
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
	}
}


