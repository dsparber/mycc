using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCC.Forms.Resources;
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

            ManualSection.Title = I18N.ManuallyAdded;
            BittrexSection.Title = I18N.BittrexAdded;
            AddressSection.Title = I18N.AddressAdded;

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

            Func<AccountRepository, CustomViewCell> GetCell = r =>
               {
                   var c = new CustomViewCell { Image = "more.png", Text = r.Name };
                   c.Tapped += (sender, e) => Navigation.PushAsync(new RepositoryView(r));
                   return c;
               };

            var manualCells = repositories.OfType<LocalAccountRepository>().SelectMany(r => r.Elements).Select(a =>
            {
                var c = new CustomViewCell { Image = "more.png", Text = a.Money.ToString(), Detail = a.Name };
                c.Tapped += (sender, e) => Navigation.PushAsync(new AccountEditView(a, AccountStorage.Instance.LocalRepository as LocalAccountRepository));
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var bittrexCells = repositories.OfType<BittrexAccountRepository>().Select(r =>
            {
                var c = GetCell(r);
                c.Detail = PluralHelper.GetTextCurrencies(r.Elements.ToList().Count);
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var addressCells = repositories.OfType<AddressAccountRepository>().Select(r =>
            {
                var c = GetCell(r);
                c.Detail = $"{I18N.Address}: {r.Address}";
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();

            Device.BeginInvokeOnMainThread(() =>
                {
                    SetHeader();

                    NoSourcesView.IsVisible = AccountStorage.Instance.AllElements.Count == 0;
                    Table.IsVisible = AccountStorage.Instance.AllElements.Count > 0;

                    if (AccountStorage.Instance.AllElements.Count <= 0) return;

                    ManualSection.Clear();
                    BittrexSection.Clear();
                    AddressSection.Clear();

                    AddressSection.Add(addressCells);
                    BittrexSection.Add(bittrexCells);
                    ManualSection.Add(manualCells);

                    if (addressCells.Count == 0)
                    {
                        Table.Root.Remove(AddressSection);
                    }
                    else
                    {
                        if (!Table.Root.Contains(AddressSection))
                        {
                            Table.Root.Add(AddressSection);
                        }
                    }

                    if (bittrexCells.Count == 0)
                    {
                        Table.Root.Remove(BittrexSection);
                    }
                    else
                    {
                        if (!Table.Root.Contains(BittrexSection))
                        {
                            Table.Root.Add(BittrexSection);
                        }
                    }

                    if (manualCells.Count == 0)
                    {
                        Table.Root.Remove(ManualSection);
                    }
                    else
                    {
                        if (!Table.Root.Contains(ManualSection))
                        {
                            Table.Root.Add(ManualSection);
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
