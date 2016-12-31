using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
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