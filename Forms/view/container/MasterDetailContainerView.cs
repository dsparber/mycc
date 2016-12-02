using System.Collections.Generic;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.pages;
using MyCryptos.Forms.view.pages.settings;
using Xamarin.Forms;

namespace view
{
    public class MasterDetailContainerView : MasterDetailPage
    {
        MasterPage masterPage;

        public MasterDetailContainerView()
        {

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = I18N.Coins,
                IconSource = "coins.png",
                Page = new MyCryptos.Forms.view.pages.CoinsView()
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = I18N.Sources,
                IconSource = "accounts.png",
                Page = new SourcesView()
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = I18N.Settings,
                IconSource = "settings.png",
                Page = new SettingsView()
            });

            masterPage = new MasterPage(masterPageItems);
            Master = masterPage;
            Detail = new NavigationPage(masterPageItems[0].Page);

            masterPage.ListView.ItemSelected += OnItemSelected;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                Detail = new NavigationPage(item.Page);
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }

        class MasterPageItem
        {
            public string Title { get; set; }
            public string IconSource { get; set; }
            public Page Page { get; set; }
        }

        class MasterPage : ContentPage
        {
            public ListView ListView { get { return listView; } }
            readonly ListView listView;

            public MasterPage(List<MasterPageItem> masterPageItems)
            {
                listView = new ListView
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
                    Children = { listView }
                };
            }
        }
    }
}
