using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Overlays;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings.Source
{
    public partial class SourcesView
    {
        private List<AccountRepository> _repositories;

        public SourcesView()
        {
            InitializeComponent();

            SetView();

            ManualSection.Title = I18N.ManuallyAdded;
            BittrexSection.Title = string.Format(I18N.AddedWith, ConstantNames.Bittrex);
            PoloniexSection.Title = string.Format(I18N.AddedWith, ConstantNames.Poloniex);
            AddressSection.Title = I18N.AddressAdded;

            Messaging.Update.Balances.Subscribe(this, SetView);
            Messaging.Modified.Balances.Subscribe(this, SetView);
        }

        private void SetHeader()
        {
            Header.Info = AccountsText(AccountStorage.Instance.AllElements.Count);
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
                var c = new CustomViewCell { Image = "more.png", Text = $"{a.Money.Currency.Code} - {a.Name}", Detail = $"{a.Money.ToString(false)} {a.Money.Currency.FindName()}" };
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
            var poloniexCells = _repositories.OfType<PoloniexAccountRepository>().Select(r =>
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
                c.Detail = $"{r.Currency.FindName()} ({(r is BlockchainXpubAccountRepository ? r.Address.Substring(0, 4) : r.Address.MiddleTruncate())})";
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
                    PoloniexSection.Clear();
                    AddressSection.Clear();

                    AddressSection.Add(addressCells);
                    BittrexSection.Add(bittrexCells);
                    PoloniexSection.Add(poloniexCells);
                    ManualSection.Add(manualCells);

                    AddressSection.Title = $"{I18N.AddressAdded}: {AccountStorage.AddressRepositories.Count()}";
                    BittrexSection.Title = $"{string.Format(I18N.AddedWith, ConstantNames.Bittrex)}: {AccountStorage.BittrexRepositories.SelectMany(r => r.Elements).Count()}";
                    PoloniexSection.Title = $"{string.Format(I18N.AddedWith, ConstantNames.Poloniex)}: {AccountStorage.PoloniexRepositories.SelectMany(r => r.Elements).Count()}";
                    ManualSection.Title = $"{I18N.ManuallyAdded}: {AccountStorage.ManuallyAddedAccounts.Count()}";

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
                    if (poloniexCells.Count == 0)
                    {
                        Table.Root.Remove(PoloniexSection);
                    }
                    else
                    {
                        if (!Table.Root.Contains(PoloniexSection))
                        {
                            Table.Root.Add(PoloniexSection);
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
            Navigation.PushOrPushModal(new AddSourceOverlay());
        }
    }
}
