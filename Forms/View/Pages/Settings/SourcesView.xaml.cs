using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
    public partial class SourcesView
    {
        private List<AccountRepository> _repositories;

        public SourcesView()
        {
            InitializeComponent();

            SetView();

            ManualSection.Title = I18N.ManuallyAdded;
            BittrexSection.Title = I18N.BittrexAdded;
            AddressSection.Title = I18N.AddressAdded;

            Messaging.Loading.SubscribeFinished(this, SetView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetView);
        }

        private void SetHeader()
        {
            var sources = _repositories.Count - (AnyLocalAccounts ? 0 : 1);
            var local = _repositories.Where(r => r is LocalAccountRepository).ToList().Count - (AnyLocalAccounts ? 0 : 1);

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

        private static Func<int, string> AccountsText => (count) => PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);

        private void SetView()
        {
            _repositories = AccountStorage.Instance.Repositories ?? new List<AccountRepository>();

            Func<AccountRepository, CustomViewCell> GetCell = r =>
               {
                   var c = new CustomViewCell { Image = "more.png", Text = r.Name };
                   c.Tapped += (sender, e) => Navigation.PushAsync(new RepositoryView(r));
                   return c;
               };

            var manualCells = _repositories.OfType<LocalAccountRepository>().SelectMany(r => r.Elements).Select(a =>
            {
                var c = new CustomViewCell { Image = "more.png", Text = a.Money.ToString(), Detail = a.Name };
                c.Tapped += (sender, e) => Navigation.PushAsync(new AccountEditView(a, AccountStorage.Instance.LocalRepository as LocalAccountRepository));
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var bittrexCells = _repositories.OfType<BittrexAccountRepository>().Select(r =>
            {
                var c = GetCell(r);
                c.Detail = PluralHelper.GetTextAccounts(r.Elements.ToList().Count);
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var addressCells = _repositories.OfType<AddressAccountRepository>().Select(r =>
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
            NavigationHelper.PushOrPushModal(Navigation, new AddSourceView());
        }
    }
}
