using System.Collections.Generic;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Resources;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Addsource
{
    public sealed class AddBittrexSubview : AddRepositorySubview
    {

        private readonly TableSection _section;
        private readonly CustomEntryCell _apiKeyEntryCell, _apiPrivateKeyEntryCell;

        public AddBittrexSubview()
        {
            _apiKeyEntryCell = new CustomEntryCell { Title = I18N.ApiKey, Placeholder = I18N.ApiKey };
            _apiPrivateKeyEntryCell = new CustomEntryCell { Title = I18N.ApiSecret, Placeholder = I18N.ApiSecret };

            _section = new TableSection { Title = I18N.Details };
            _section.Add(_apiKeyEntryCell);
            _section.Add(_apiPrivateKeyEntryCell);
        }

        public override OnlineAccountRepository GetRepository(string name)
        {
            var key = _apiKeyEntryCell.Text ?? string.Empty;
            var secretKey = _apiPrivateKeyEntryCell.Text ?? string.Empty;

            var repository = new BittrexAccountRepository(default(int), name, key, secretKey);
            return repository;
        }

        public override bool Enabled
        {
            set
            {
                _apiKeyEntryCell.IsEditable = value;
                _apiPrivateKeyEntryCell.IsEditable = value;
            }
        }

        public override string Description => ConstantNames.Bittrex;

        public override List<TableSection> InputSections => new List<TableSection> { _section };

        public override void Unfocus()
        {
            _apiKeyEntryCell.Entry.Unfocus();
            _apiPrivateKeyEntryCell.Entry.Unfocus();
        }
    }
}
