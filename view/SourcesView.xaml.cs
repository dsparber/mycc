using System;
using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using MyCryptos.resources;
using MyCryptos.view.components;
using message;
using Xamarin.Forms;
using data.storage;

namespace view
{
    public partial class SourcesView : ContentPage
    {
        List<AccountRepository> repositories;

        public SourcesView(List<AccountRepository> repositories)
        {
            InitializeComponent();
            this.repositories = repositories;

            setView();

            if (Device.OS == TargetPlatform.Android)
            {
                ToolbarItems.Remove(DoneItem);
            }

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, async str =>
            {
                this.repositories = await AccountStorage.Instance.Repositories();
                setView();
            });
        }

        public async void Done(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        void setHeader()
        {
            var sources = repositories.Count;
            var local = repositories.Where(r => r is LocalAccountRepository).ToList().Count;

            Header.TitleText = (sources == 0) ?
                InternationalisationResources.NoSources :
                string.Format("{0} {1}", sources, ((sources == 1) ?
                                                   InternationalisationResources.Source :
                                                   InternationalisationResources.Sources));

            Header.InfoText = string.Format("{0} {1}, {2} {3}", local, InternationalisationResources.Local, (sources - local), InternationalisationResources.Online);
        }

        void setView()
        {
            setHeader();

            LocalSection.Clear();
            OnlineSection.Clear();

            foreach (var r in repositories)
            {
                var c = new CustomViewCell { Text = r.Name, Detail = r.Type, Image = "more.png" };
                c.Tapped += (sender, e) => Navigation.PushAsync(new RepositoryView(r));

                if (r is LocalAccountRepository)
                {
                    LocalSection.Add(c);
                }
                else
                {
                    OnlineSection.Add(c);
                }
            }

            var cell = new CustomViewCell { Text = InternationalisationResources.AddSource, IsActionCell = true };
            cell.Tapped += (sender, e) =>
            {
                if (Device.OS == TargetPlatform.Android)
                {
                    Navigation.PushAsync((new AddRepositoryView()));
                }
                else
                {
                    Navigation.PushModalAsync(new NavigationPage(new AddRepositoryView()));
                }
            };
            OnlineSection.Add(cell);
        }
    }
}
