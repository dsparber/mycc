using MyCryptos.resources;
using Xamarin.Forms;
using System;

namespace view
{
	public class TabContainerView : TabbedPage
	{
		public TabContainerView()
		{
			Title = InternationalisationResources.AppName;
			var tagPage = new NavigationPage(new TagsView()) { Title = InternationalisationResources.TagsTitle, Icon = "tags.png" };
			var coinPage = new NavigationPage(new CoinsView()) { Title = InternationalisationResources.CoinsTitle, Icon = "coins.png" };
			var accountPage = new NavigationPage(new AccountsView()) { Title = InternationalisationResources.AccountsTitle, Icon = "accounts.png" };
			var settingsPage = new NavigationPage(new SettingsView()) { Title = InternationalisationResources.SettingsTitle, Icon = "settings.png" };

			Children.Add(tagPage);
			Children.Add(coinPage);
			Children.Add(accountPage);
			if (Device.OS == TargetPlatform.Android)
			{
				var settingsToolbarItem = new ToolbarItem { Text = InternationalisationResources.Settings };
				settingsToolbarItem.Clicked += (sender, e) => Navigation.PushModalAsync(settingsPage);
				ToolbarItems.Add(settingsToolbarItem);
			}
			else {
				Children.Add(settingsPage);
			}

			CurrentPage = coinPage;
		}
	}
}


