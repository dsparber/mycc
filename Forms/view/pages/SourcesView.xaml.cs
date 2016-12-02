using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using view;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
    public partial class SourcesView
    {
        private List<AccountRepository> repositories;

        public SourcesView()
        {
            InitializeComponent();

            SetView();

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }

            Messaging.Loading.SubscribeFinished(this, SetView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetView);
        }

        private void SetHeader()
        {
            var sources = repositories.Count;
            var local = repositories.Where(r => r is LocalAccountRepository).ToList().Count;

            Header.TitleText = AccountsText(AccountStorage.Instance.AllElements.Count);
            Func<int, string> sourcesText = (count) => PluralHelper.GetText(I18N.NoSources, I18N.OneSource, I18N.Sources, count);
            var localOnlineText = string.Empty;

            if (local >= 1 && (sources - local) >= 1)
            {
                localOnlineText = $" ({local} {I18N.Local}, {(sources - local)} {I18N.Online})";
            }
            else if (local >= 1)
            {
                localOnlineText = local == 1 ? $" ({I18N.Local})" : $" ({local} {I18N.Local})";
            }
            else if ((sources - local) >= 1)
            {
                localOnlineText = (sources - local) == 1 ? $" ({I18N.Online})" : $" ({(sources - local)} {I18N.Online})";
            }

            Header.InfoText = $"{sourcesText(sources)}{localOnlineText}";
        }

        private Func<int, string> AccountsText => (count) => PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);

        private void SetView()
        {
            repositories = AccountStorage.Instance.Repositories ?? new List<AccountRepository>();

            SetHeader();

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

        private void Add(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceView());
        }

        private void SetLoadingAnimation(bool loading)
        {
            IsBusy = loading;
            Header.IsLoading = loading;
        }
    }
}
