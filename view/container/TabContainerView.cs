using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public class TabContainerView : TabbedPage
	{
		public TabContainerView()
		{
			Title = InternationalisationResources.AppName;
			//var tagPage = new NavigationPage(new TagsView()) { Title = InternationalisationResources.TagsTitle, Icon = "tags.png" };
			var coinPage = new NavigationPage(new CoinsView()) { Title = InternationalisationResources.Coins, Icon = "coins.png" };
			var accountPage = new NavigationPage(new AccountsView()) { Title = InternationalisationResources.Accounts, Icon = "accounts.png" };
			var settingsPage = new NavigationPage(new SettingsView()) { Title = InternationalisationResources.Settings, Icon = "settings.png" };

			//Children.Add(tagPage);
			Children.Add(coinPage);
			Children.Add(accountPage);
			Children.Add(settingsPage);

			CurrentPage = coinPage;
		}
	}
}