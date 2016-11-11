using System;
using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using MyCryptos.resources;
using MyCryptos.view.components;
using message;
using Xamarin.Forms;
using data.storage;
using MyCryptos.helpers;

namespace view
{
	public partial class SourcesView : ContentPage
	{
		List<AccountRepository> repositories;

		public SourcesView()
		{
			InitializeComponent();
			repositories = AccountStorage.Instance.Repositories ?? new List<AccountRepository>();

			setView();

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(DoneItem);
				Title = string.Empty;
			}

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str =>
			{
				repositories = AccountStorage.Instance.Repositories;
				setView();
			});
		}

		public async void Done(object sender, EventArgs e)
		{
			await Navigation.PopOrPopModal();
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
				var c = new CustomViewCell { Text = r.Name, Detail = r.Description, Image = "more.png" };
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
			cell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddRepositoryView());
			OnlineSection.Add(cell);
		}
	}
}
