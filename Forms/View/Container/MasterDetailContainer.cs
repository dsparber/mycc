using System;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Pages;
using MyCC.Forms.View.Pages.Settings;
using Xamarin.Forms;
using HeaderView = MyCC.Forms.View.Components.Header.HeaderView;

namespace MyCC.Forms.View.Container
{
    public class MasterDetailContainer : MasterDetailPage
    {
        private readonly Page _ratesPage;
        private readonly Page _coinGraphPage;
        private readonly Page _coinTablePage;
        private readonly Page _settingsPage;

        public MasterDetailContainer()
        {
            Title = I18N.AppName;
            BackgroundColor = AppConstants.TableBackgroundColor;

            _ratesPage = new NavigationPage(new RateView()) { Title = I18N.Rates, Icon = "rate.png", BarTextColor = Color.White };
            _coinGraphPage = new NavigationPage(new AssetsGraphView()) { Title = I18N.Graph, Icon = "graph.png", BarTextColor = Color.White };
            _coinTablePage = new NavigationPage(new AssetsTableView()) { Title = I18N.Table, Icon = "table.png", BarTextColor = Color.White };
            _settingsPage = new NavigationPage(new SettingsView()) { Title = I18N.Settings, Icon = "settings.png", BarTextColor = Color.White };

            Master = new MasterPage(this);
            Detail = ApplicationSettings.DefaultPage == StartupPage.GraphView ? _coinGraphPage : ApplicationSettings.DefaultPage == StartupPage.TableView ? _coinTablePage : _ratesPage;
        }

        private class MasterPage : ContentPage
        {

            public MasterPage(MasterDetailContainer container)
            {
                Icon = "hamburger.png";
                Title = I18N.AppName;
                BackgroundColor = Color.White;

                var stack = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 0
                };

                Func<string, string, Page, Xamarin.Forms.View> item = (title, icon, page) => new ContentView
                {
                    BackgroundColor = AppConstants.BorderColor,
                    Padding = new Thickness(0, 0, 0, 1),
                    Content = new StackLayout
                    {
                        BackgroundColor = Color.White,
                        Padding = 10,
                        Spacing = 10,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Image {Source = icon},
                            new Label {Text = title, VerticalOptions = LayoutOptions.Center, TextColor = AppConstants.FontColor},
                        },
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer {Command = new Command(() =>
                            {
                                container.Detail = page;
                                container.IsPresented = false;
                            })}
                        }
                    }
                };

                stack.Children.Add(item(I18N.Rates, "rate.png", container._ratesPage));
                stack.Children.Add(item($"{I18N.Assets} ({I18N.Table})", "table.png", container._coinTablePage));
                stack.Children.Add(item($"{I18N.Assets} ({I18N.Graph})", "graph.png", container._coinGraphPage));
                stack.Children.Add(item(I18N.Settings, "settings.png", container._settingsPage));

                Content = new StackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        new HeaderView { TitleText = I18N.AppName, InfoText = I18N.AppNameLong },
                        new ScrollView { Content = stack }
                    }
                };
            }
        }
    }
}