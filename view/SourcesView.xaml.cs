using System;
using System.Collections.Generic;
using System.Linq;
using data.repositories.account;
using MyCryptos.resources;
using view.components;
using Xamarin.Forms;

namespace view
{
	public partial class SourcesView : ContentPage
	{
		List<AccountRepository> repositories;

		public SourcesView(List<AccountRepository> repositories)
		{
			InitializeComponent();
			this.repositories = repositories;

			setHeader();

			foreach (var r in repositories)
			{
				var c = new CustomViewCell { Text = r.Name, Detail = r.Type, Image = "more.png" };

				if (r is LocalAccountRepository)
				{
					LocalSection.Add(c);
				}
				else {
					OnlineSection.Add(c);
				}
			}

			var cell = new CustomViewCell { Text = InternationalisationResources.AddSource, IsActionCell = true };
			cell.Tapped += (sender, e) => Navigation.PushModalAsync(new NavigationPage(new AddRepositoryView()));
			OnlineSection.Add(cell);
		}

		public async void Cancel(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		public async void Save(object sender, EventArgs e)
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
	}
}
