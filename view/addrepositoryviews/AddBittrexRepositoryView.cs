using data.repositories.account;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.resources;

namespace MyCryptos.view.addrepositoryviews
{
	public sealed class AddBittrexRepositoryView : AbstractAddRepositoryView
	{

		readonly TableSection section;
		CustomEntryCell apiKeyEntryCell, apiPrivateKeyEntryCell;

		public AddBittrexRepositoryView()
		{
			apiKeyEntryCell = new CustomEntryCell { Title = InternationalisationResources.ApiKey, Placeholder = InternationalisationResources.ApiKey };
			apiPrivateKeyEntryCell = new CustomEntryCell { Title = InternationalisationResources.SecretApiKey, Placeholder = InternationalisationResources.SecretApiKey };

			section = new TableSection();
			section.Title = InternationalisationResources.GrantAccess;
			section.Add(apiKeyEntryCell);
			section.Add(apiPrivateKeyEntryCell);
		}

		public override OnlineAccountRepository GetRepository(string name)
		{
			var key = apiKeyEntryCell.Text ?? string.Empty;
			var secretKey = apiPrivateKeyEntryCell.Text ?? string.Empty;

			var repository = new BittrexAccountRepository(name, key, secretKey);
			return repository;
		}

		public override bool Enabled
		{
			set
			{
				apiKeyEntryCell.IsEditable = value;
				apiPrivateKeyEntryCell.IsEditable = value;
			}
		}

		public override string DefaultName
		{
			get { return InternationalisationResources.BittrexAccount; }
		}

		public override string Description
		{
			get { return InternationalisationResources.Bittrex; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
