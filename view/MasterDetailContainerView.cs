using MyCryptos.resources;
using System.Collections.Generic;
using Xamarin.Forms;

namespace view
{
    class MasterDetailContainerView : MasterDetailPage
    {
        MasterPage masterPage;

        public MasterDetailContainerView()
        {

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = InternationalisationResources.CoinsTitle,
                IconSource = "coins.png",
                Page = new CoinsView()
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = InternationalisationResources.AccountsTitle,
                IconSource = "accounts.png",
                Page = new AccountsView()
            });
			masterPageItems.Add(new MasterPageItem
			{
				Title = InternationalisationResources.Sources,
				IconSource = "sources.png",
				Page = new SourcesView()
			});
            masterPageItems.Add(new MasterPageItem
            {
                Title = InternationalisationResources.Settings,
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
                Title = InternationalisationResources.AppName;
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = { listView }
                };
            }
        }
    }
}
