using System.Collections.Generic;
using System.Linq;
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

		IEnumerable<Account> accounts;
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
		public IEnumerable<Account> Accounts
		{
			get
			{
				if (accounts == null)
				{
					accounts = new List<Account>();
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

		public Currency Currency
		{
			get
			{
				return Accounts.ToList().Count > 0 ? Accounts.First().Money.Currency : null;
			}
		}

		Money moneySum
		{
			get
			{
				if (Accounts.ToList().Count > 0)
				{
					return new Money(Accounts.Sum(a => a.Money.Amount), Accounts.First().Money.Currency);
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
				Navigation.PushAsync(new CoinDetailView(accounts, exchangeRate));
			};
			if (View != null)
			{
				View.GestureRecognizers.Clear();
				View.GestureRecognizers.Add(gestureRecognizer);
			}
		}
	}
}


