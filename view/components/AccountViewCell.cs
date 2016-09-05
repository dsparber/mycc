using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view.components
{
	public class AccountViewCell : ViewCell
	{
		readonly Label AccountNameLabel;
		readonly Label MoneyLabel;

		readonly INavigation Navigation;

		Account account;

		public Account Account
		{
			get
			{
				return account;
			}
			set
			{
				account = value;
				AccountNameLabel.Text = account.Name;
				MoneyLabel.Text = account.Money.ToString();
				setTapRecognizer();
			}
		}

		public AccountRepository repository;

		public AccountRepository Repository
		{
			get { return repository; }
			set { repository = value; setTapRecognizer(); }
		}

		public bool IsLoading
		{
			set
			{
				if (value)
				{
					MoneyLabel.Text = InternationalisationResources.RefreshingDots;
				}
				else {
					MoneyLabel.Text = account.Money.ToString();
				}
			}
		}

		public AccountViewCell(INavigation navigation)
		{
			Navigation = navigation;

			AccountNameLabel = new Label();
			MoneyLabel = new Label();

			MoneyLabel.TextColor = Color.Gray;
			MoneyLabel.FontSize = AccountNameLabel.FontSize * 0.75;

			var stack = new StackLayout();
			stack.Spacing = 0;
			stack.Children.Add(AccountNameLabel);
			stack.Children.Add(MoneyLabel);

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
				Navigation.PushAsync(new AccountDetailView(account, repository));
			};
			if (View != null)
			{
				View.GestureRecognizers.Clear();
				View.GestureRecognizers.Add(gestureRecognizer);
			}
		}
	}
}


