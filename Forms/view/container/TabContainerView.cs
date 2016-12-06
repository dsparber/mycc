using MyCryptos.Core.settings;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.pages;
using MyCryptos.Forms.view.pages.settings;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.container
{
    public class TabContainerView : TabbedPage
    {
        public TabContainerView()
        {
            Title = I18N.AppName;
            var coinPage = new NavigationPage(new CoinsView()) { Title = I18N.Coins, Icon = "coins.png", BarTextColor = Color.White };
            var sourcesPage = new NavigationPage(new SourcesView()) { Title = I18N.Sources, Icon = "accounts.png", BarTextColor = Color.White };
            var settingsPage = new NavigationPage(new SettingsView()) { Title = I18N.Settings, Icon = "settings.png", BarTextColor = Color.White };

            Children.Add(coinPage);
            Children.Add(sourcesPage);
            Children.Add(settingsPage);

            CurrentPage = ApplicationSettings.FirstLaunch ? sourcesPage : coinPage;
        }
    }
}