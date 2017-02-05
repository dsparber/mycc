using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Pages;
using MyCC.Forms.View.Pages.Settings;
using Xamarin.Forms;

namespace MyCC.Forms.View.Container
{
    public class MasterDetailContainerView : MasterDetailPage
    {
        private readonly MasterPage masterPage;

        public MasterDetailContainerView()
        {

            var masterPageItems = new List<MasterPageItem>
            {
                new MasterPageItem
                {
                    Title = I18N.Graph,
                    IconSource = "graph.png",
                    Page = new CoinGraphView()
                },new MasterPageItem
                {
                    Title = I18N.Table,
                    IconSource = "table.png",
                    Page = new CoinTableView()
                },
                new MasterPageItem
                {
                    Title = I18N.Sources,
                    IconSource = "accounts.png",
                    Page = new SourcesView()
                },
                new MasterPageItem
                {
                    Title = I18N.Settings,
                    IconSource = "settings.png",
                    Page = new SettingsView()
                }
            };

            masterPage = new MasterPage(masterPageItems);
            Master = masterPage;
            Detail = new NavigationPage(masterPageItems[ApplicationSettings.FirstLaunch ? 1 : 0].Page);

            masterPage.ListView.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item == null) return;

            Detail = new NavigationPage(item.Page);
            masterPage.ListView.SelectedItem = null;
            IsPresented = false;
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class MasterPageItem
        {
            public string Title { get; set; }
            public string IconSource { get; set; }
            public Page Page { get; set; }
        }

        private class MasterPage : ContentPage
        {
            public ListView ListView { get; }

            public MasterPage(IEnumerable<MasterPageItem> masterPageItems)
            {
                ListView = new ListView
                {
                    ItemsSource = masterPageItems,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var imageCell = new ImageCell();
                        imageCell.SetBinding(TextCell.TextProperty, "Title");
                        imageCell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");
                        imageCell.TextColor = Color.Black;

                        return imageCell;
                    }),
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                Icon = "hamburger.png";
                Title = I18N.AppName;
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = { ListView }
                };
            }
        }
    }
}
