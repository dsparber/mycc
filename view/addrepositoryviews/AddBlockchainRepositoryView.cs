using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using data.repositories.currency;

namespace MyCryptos.view.addrepositoryviews
{
	public sealed class AddBlockchainRepositoryView : AbstractAddRepositoryView
	{

		readonly TableSection section;
		CustomEntryCell addressEntryCell;

		public AddBlockchainRepositoryView()
		{
			addressEntryCell = new CustomEntryCell { Title = InternationalisationResources.Address, Placeholder = InternationalisationResources.Address };

			section = new TableSection();

			section.Title = InternationalisationResources.AccountInformation;
			section.Add(addressEntryCell);
		}

		public override AccountRepository GetRepository(string name)
		{
			var address = addressEntryCell.Text ?? string.Empty;

			var repository = new BlockchainAccountRepository(name, address);
			return repository;
		}

		public override bool Enabled
		{
			set
			{
				addressEntryCell.IsEditable = value;
			}
		}

		public override string DefaultName
		{
			get { return InternationalisationResources.BlockchainAccount; }
		}

		public override string Description
		{
			get { return InternationalisationResources.Blockchain; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
