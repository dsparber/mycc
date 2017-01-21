using System.Collections.Generic;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Forms.Resources;
using MyCC.Forms.view.components.cells;
using Xamarin.Forms;

namespace MyCC.Forms.view.addsource
{
    public sealed class AddBittrexSubview : AddRepositorySubview
    {

        private readonly TableSection section;
        private readonly CustomEntryCell apiKeyEntryCell, apiPrivateKeyEntryCell;

        public AddBittrexSubview()
        {
            apiKeyEntryCell = new CustomEntryCell { Title = I18N.ApiKey, Placeholder = I18N.ApiKey };
            apiPrivateKeyEntryCell = new CustomEntryCell { Title = I18N.SecretApiKey, Placeholder = I18N.SecretApiKey };

            section = new TableSection { Title = I18N.GrantAccess };
            section.Add(apiKeyEntryCell);
            section.Add(apiPrivateKeyEntryCell);
        }

        public override OnlineAccountRepository GetRepository(string name)
        {
            var key = apiKeyEntryCell.Text ?? string.Empty;
            var secretKey = apiPrivateKeyEntryCell.Text ?? string.Empty;

            var repository = new BittrexAccountRepository(default(int), name, key, secretKey);
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

        public override string Description => I18N.Bittrex;

        public override List<TableSection> InputSections => new List<TableSection> { section };

        public override void Unfocus()
        {
            apiKeyEntryCell.Entry.Unfocus();
            apiPrivateKeyEntryCell.Entry.Unfocus();
        }
    }
}
