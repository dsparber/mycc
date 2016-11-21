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
			apiKeyEntryCell = new CustomEntryCell { Title = I18N.ApiKey, Placeholder = I18N.ApiKey };
			apiPrivateKeyEntryCell = new CustomEntryCell { Title = I18N.SecretApiKey, Placeholder = I18N.SecretApiKey };

			section = new TableSection();
			section.Title = I18N.GrantAccess;
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
			get { return I18N.BittrexAccount; }
		}

		public override string Description
		{
			get { return I18N.Bittrex; }
		}

		public override TableSection InputSection
		{
			get { return section; }
		}
	}
}
