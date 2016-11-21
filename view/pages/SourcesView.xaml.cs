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
using tasks;

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
				Title = string.Empty;
			}

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str =>
			{
				repositories = AccountStorage.Instance.Repositories;
				setView();
			});
		}

		void setHeader()
		{
			var sources = repositories.Count;
			var local = repositories.Where(r => r is LocalAccountRepository).ToList().Count;

			Header.TitleText = AccountsText(AccountStorage.Instance.AllElements.Count);
			Func<int, string> sourcesText = (count) => PluralHelper.GetText(I18N.NoSources, I18N.OneSource, I18N.Sources, count);
			string localOnlineText = string.Empty;

			if (local >= 1 && (sources - local) >= 1)
			{
				localOnlineText = $" ({local} {I18N.Local}, {(sources - local)} {I18N.Online})";
			}
			else if (local >= 1)
			{
				if (local == 1)
				{
					localOnlineText = $" ({I18N.Local})";
				}
				else {
					localOnlineText = $" ({local} {I18N.Local})";
				}
			}
			else if ((sources - local) >= 1)
			{
				if ((sources - local) == 1)
				{
					localOnlineText = $" ({I18N.Online})";
				}
				else {
					localOnlineText = $" ({(sources - local)} {I18N.Online})";
				}
			}

			Header.InfoText = $"{sourcesText(sources)}{localOnlineText}";
		}

		private Func<int, string> AccountsText => (count) => PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);

		void setView()
		{
			setHeader();

			LocalSection.Clear();
			OnlineSection.Clear();

			foreach (var r in repositories)
			{
				var c = new CustomViewCell { Text = r.Name, Detail = $"{AccountsText(r.Elements.ToList().Count)} | {I18N.Type}: {r.Description}", Image = "more.png" };
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

			var cell = new CustomViewCell { Text = I18N.AddSource, IsActionCell = true };
			cell.Tapped += Add;
			OnlineSection.Add(cell);
		}

		void Add(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new AddRepositoryView());
		}

		void Refresh(object sender, EventArgs e)
		{
			// TODO Only fetch accounts
			AppTasks.Instance.StartFetchTask(false);
		}
	}
}
