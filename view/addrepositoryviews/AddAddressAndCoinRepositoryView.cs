using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using System;
using MyCryptos.models;

namespace MyCryptos.view.addrepositoryviews
{
    public abstract class AddAddressAndCoinRepositoryView : AbstractAddRepositoryView
    {

        readonly TableSection section;
        CurrencyEntryCell currencyEntryCell;
        CustomEntryCell addressEntryCell;

        public AddAddressAndCoinRepositoryView(INavigation navigation, Type type, string name)
        {
            Name = name;

            currencyEntryCell = new CurrencyEntryCell(navigation) { IsAmountEnabled = false, CurrencyRepositoryType = type, IsFormRepresentation = true };
            addressEntryCell = new CustomEntryCell { Title = InternationalisationResources.Address, Placeholder = InternationalisationResources.Address };

            section = new TableSection();

            section.Title = InternationalisationResources.AccountInformation;
            section.Add(currencyEntryCell);
            section.Add(addressEntryCell);
        }

        public override OnlineAccountRepository GetRepository(string name)
        {
            var coin = currencyEntryCell.SelectedCurrency;
            var address = addressEntryCell.Text ?? string.Empty;

            var repository = GetRepository(name, coin, address);
            return repository;
        }

        protected abstract OnlineAccountRepository GetRepository(string name, Currency coin, string address);

        public override bool Enabled
        {
            set
            {
                currencyEntryCell.IsEditable = value;
                addressEntryCell.IsEditable = value;
            }
        }

        public override TableSection InputSection
        {
            get { return section; }
        }

        public sealed override string DefaultName
        {
            get { return Name; }
        }

        public sealed override string Description
        {
            get { return Name; }
        }

        string Name;
    }
}
