using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
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
            Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetView);
            Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetView);
        }

        private void SetHeader()
        {
            var sources = repositories.Count - (AnyLocalAccounts ? 0 : 1);
            var local = repositories.Where(r => r is LocalAccountRepository).ToList().Count - (AnyLocalAccounts ? 0 : 1);

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
            Header.IsLoading = false;
        }

        private static Func<int, string> AccountsText => (count) => PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);

        private void SetView()
        {
            repositories = AccountStorage.Instance.Repositories ?? new List<AccountRepository>();

            var onlineCells = new List<CustomViewCell>();
            var localCells = new List<CustomViewCell>();

            foreach (var r in repositories)
            {
                var c = new CustomViewCell { Text = r.Name, Detail = $"{AccountsText(r.Elements.ToList().Count)} | {I18N.Type}: {r.Description}", Image = "more.png" };
                c.Tapped += (sender, e) => Navigation.PushAsync(new RepositoryView(r));

                if (r is LocalAccountRepository)
                {
                    localCells.Add(c);
                }
                else
                {
                    onlineCells.Add(c);
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                SetHeader();

                NoSourcesView.IsVisible = AccountStorage.Instance.AllElements.Count == 0;
                Table.IsVisible = AccountStorage.Instance.AllElements.Count > 0;

                if (AccountStorage.Instance.AllElements.Count <= 0) return;

                LocalSection.Clear();
                OnlineSection.Clear();
                LocalSection.Add(localCells);
                OnlineSection.Add(onlineCells);

                if (onlineCells.Count == 0)
                {
                    Table.Root.Remove(OnlineSection);
                }
                else
                {
                    if (!Table.Root.Contains(OnlineSection))
                    {
                        Table.Root.Add(OnlineSection);
                    }
                }
                if (localCells.Count == 0)
                {
                    Table.Root.Remove(LocalSection);
                }
                else
                {
                    if (!Table.Root.Contains(LocalSection))
                    {
                        Table.Root.Add(LocalSection);
                    }
                }
            });
        }

        private static bool AnyLocalAccounts => AccountStorage.Instance.LocalRepository?.Elements.ToList().Count > 0;

        private void Add(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceView());
        }
    }
}
