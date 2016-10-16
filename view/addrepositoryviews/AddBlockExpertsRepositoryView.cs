using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using data.repositories.currency;

namespace MyCryptos.view.addrepositoryviews
{
	public sealed class AddBlockExpertsRepositoryView : AbstractAddRepositoryView
	{

		readonly TableSection section;
		CurrencyEntryCell currencyEntryCell;
		CustomEntryCell addressEntryCell;

		public AddBlockExpertsRepositoryView(INavigation navigation)
		{
			currencyEntryCell = new CurrencyEntryCell(navigation) { IsAmountEnabled = false, CurrencyRepositoryType = typeof(BlockExpertsCurrencyRepository), IsFormRepresentation = true };
			addressEntryCell = new CustomEntryCell { Title = InternationalisationResources.Address, Placeholder = InternationalisationResources.Address };

			section = new TableSection();

			section.Title = InternationalisationResources.AccountInformation;
			section.Add(currencyEntryCell);
			section.Add(addressEntryCell);
		}

		public override AccountRepository GetRepository(string name)
		{
			var coin = currencyEntryCell.SelectedCurrency;
			var address = addressEntryCell.Text ?? string.Empty;

			var repository = new BlockExpertsAccountRepository(name, coin, address);
			return repository;
		}

		public override bool Enabled
		{
			set
			{
				currencyEntryCell.IsEditable = value;
				addressEntryCell.IsEditable = value;
			}
		}

		public override string DefaultName
		{
			get { return InternationalisationResources.BlockExpertsAccount; }
		}

		public override string Description
		{
			get { return InternationalisationResources.BlockExperts; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
