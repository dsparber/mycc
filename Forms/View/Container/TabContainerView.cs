using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Pages;
using MyCC.Forms.View.Pages.Settings;
using Xamarin.Forms;

namespace MyCC.Forms.View.Container
{
    public class TabContainerView : TabbedPage
    {
        public TabContainerView()
        {
            Title = I18N.AppName;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var ratesPage = new NavigationPage(new RateView()) { Title = I18N.Rates, Icon = "rate.png", BarTextColor = Color.White };
            var coinGraphPage = new NavigationPage(new CoinGraphView()) { Title = I18N.Graph, Icon = "graph.png", BarTextColor = Color.White };
            var coinTablePage = new NavigationPage(new CoinTableView()) { Title = I18N.Table, Icon = "table.png", BarTextColor = Color.White };
            var settingsPage = new NavigationPage(new SettingsView()) { Title = I18N.Settings, Icon = "settings.png", BarTextColor = Color.White };

            Children.Add(ratesPage);
            Children.Add(coinTablePage);
            Children.Add(coinGraphPage);
            Children.Add(settingsPage);

            CurrentPage = ApplicationSettings.DefaultPage == StartupPage.GraphView ? coinGraphPage : ApplicationSettings.DefaultPage == StartupPage.TableView ? coinTablePage : ratesPage;
        }
    }
}