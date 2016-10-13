using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using models;

namespace MyCryptos.view.addrepositoryviews
{
	public sealed class AddBlockExpertsRepositoryView : AbstractAddRepositoryView
	{

		readonly TableSection section;
		CustomEntryCell coinEntryCell, addressEntryCell;

		public AddBlockExpertsRepositoryView()
		{
			coinEntryCell = new CustomEntryCell { Title = InternationalisationResources.Currency, Placeholder = InternationalisationResources.Currency };
			addressEntryCell = new CustomEntryCell { Title = InternationalisationResources.Address, Placeholder = InternationalisationResources.Address };

			section = new TableSection();

			section.Title = InternationalisationResources.AccountInformation;
			section.Add(coinEntryCell);
			section.Add(addressEntryCell);
		}

		public override AccountRepository GetRepository(string name)
		{
			var coin = new Currency(coinEntryCell.Text ?? string.Empty);
			var address = addressEntryCell.Text ?? string.Empty;

			var repository = new BlockExpertsAccountRepository(name, coin, address);
			return repository;
		}

		public override bool Enabled
		{
			set
			{
				coinEntryCell.IsEditable = value;
				addressEntryCell.IsEditable = value;
			}
		}

		public override string DefaultName
		{
			get { return InternationalisationResources.BlockExperts; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
