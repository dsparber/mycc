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
            Header.InfoText = AccountsText(AccountStorage.Instance.AllElements.Count);
        }

        private static Func<int, string> AccountsText => count => PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, count);

        private void SetView()
        {
            _repositories = AccountStorage.Instance.Repositories ?? new List<AccountRepository>();

            Func<AccountRepository, CustomViewCell> getCell = r =>
            {
                var c = new CustomViewCell { Image = "more.png" };
                c.Tapped += (sender, e) => Navigation.PushAsync(new RepositoryView(r as OnlineAccountRepository));
                return c;
            };

            var manualCells = _repositories.OfType<LocalAccountRepository>().SelectMany(r => r.Elements).Select(a =>
            {
                var c = new CustomViewCell { Image = "more.png", Text = $"{a.Money.Currency.Code} - {a.Name}", Detail = $"{a.Money.ToString(false)} {a.Money.Currency.Name}" };
                c.Tapped += (sender, e) => Navigation.PushAsync(new AccountEditView(a));
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var bittrexCells = _repositories.OfType<BittrexAccountRepository>().Select(r =>
            {
                var c = getCell(r);
                c.Text = r.Name;
                c.Detail = PluralHelper.GetTextAccounts(r.Elements.ToList().Count);
                return c;
            }).OrderBy(c => $"{c.Text}{c.Detail}").ToList();
            var addressCells = _repositories.OfType<AddressAccountRepository>().Select(r =>
            {
                var c = getCell(r);
                c.Text = $"{r.Currency.Code} - {r.Name}";
                c.Detail = $"{r.Currency.Name} ({(r is BlockchainXpubAccountRepository ? r.Address.Substring(0, 4) : r.Address.MiddleTruncate())})";
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

                    AddressSection.Title = $"{I18N.AddressAdded} ({PluralHelper.GetTextSourcs(AccountStorage.AddressRepositories.Count())})";
                    BittrexSection.Title = $"{I18N.BittrexAdded} ({PluralHelper.GetTextSourcs(AccountStorage.BittrexRepositories.Count())})";
                    ManualSection.Title = $"{I18N.ManuallyAdded} ({PluralHelper.GetTextSourcs(AccountStorage.ManuallyAddedAccounts.Count())})";

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

        private void Add(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceView());
        }
    }
}
