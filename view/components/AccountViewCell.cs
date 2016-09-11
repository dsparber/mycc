using data.repositories.account;
using models;
using Xamarin.Forms;

namespace view.components
{
	public class AccountViewCell : CustomViewCell
	{
		readonly INavigation Navigation;

		Account account;
		AccountRepository repository;

		public Account Account
		{
			get { return account; }
			set { account = value; Text = account.Name; Detail = account.Money.ToString(); setTapRecognizer(); }
		}

		public AccountRepository Repository
		{
			get { return repository; }
			set { repository = value; setTapRecognizer(); }
		}

		public AccountViewCell(INavigation navigation)
		{
			Image = "more.png";
			Navigation = navigation;
			setTapRecognizer();
		}

		void setTapRecognizer()
		{
			var gestureRecognizer = new TapGestureRecognizer();
			gestureRecognizer.Tapped += (sender, e) => Navigation.PushAsync(new AccountDetailView(account, repository));

			if (View != null)
			{
				View.GestureRecognizers.Clear();
				View.GestureRecognizers.Add(gestureRecognizer);
			}
		}
	}
}


