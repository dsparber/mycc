using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;
using MyCryptos.data.repositories.account;

namespace MyCryptos.view.addrepositoryviews
{
	public abstract class AddAddressRepositoryView : AbstractAddRepositoryView
	{

		readonly TableSection section;
		CustomEntryCell addressEntryCell;
        protected abstract OnlineAccountRepository repository { get; }

		public AddAddressRepositoryView()
		{
			addressEntryCell = new CustomEntryCell { Title = InternationalisationResources.Address, Placeholder = InternationalisationResources.Address };

			section = new TableSection();

			section.Title = InternationalisationResources.AccountInformation;
			section.Add(addressEntryCell);
		}

		public override OnlineAccountRepository GetRepository(string name)
		{
			var address = addressEntryCell.Text ?? string.Empty;
			return GetRepository(name, address);
		}
        protected abstract OnlineAccountRepository GetRepository(string name, string address);


        public override bool Enabled
		{
			set
			{
				addressEntryCell.IsEditable = value;
			}
		}

		public override string DefaultName
		{
			get { return repository.Description; }
		}

		public override string Description
		{
			get { return repository.Description; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
