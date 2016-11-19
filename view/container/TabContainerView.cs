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
			var coinPage = new NavigationPage(new CoinsView()) { Title = InternationalisationResources.Coins, Icon = "coins.png", BarTextColor = Color.White };
			var accountPage = new NavigationPage(new AccountsView()) { Title = InternationalisationResources.Accounts, Icon = "accounts.png", BarTextColor = Color.White };
			var sourcesPage = new NavigationPage(new SourcesView()) { Title = InternationalisationResources.Sources, Icon = "sources.png", BarTextColor = Color.White };
            var settingsPage = new NavigationPage(new SettingsView()) { Title = InternationalisationResources.Settings, Icon = "settings.png", BarTextColor = Color.White };

			//Children.Add(tagPage);
			Children.Add(coinPage);
			Children.Add(accountPage);
			Children.Add(sourcesPage);
            Children.Add(settingsPage);

			CurrentPage = coinPage;
		}
	}
}