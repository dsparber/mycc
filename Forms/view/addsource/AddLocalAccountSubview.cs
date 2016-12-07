using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.addsource
{
    public class AddLocalAccountSubview : AddAccountSubview
    {
        private readonly TableSection inputSection;
        private readonly CurrencyEntryCell currencyCell;
        private readonly CustomEntryCell amountEntryCell;

        public AddLocalAccountSubview(INavigation navigation)
        {
            inputSection = new TableSection { Title = I18N.AmountAndCurrency };
            amountEntryCell = new CustomEntryCell { Title = I18N.Amount, Placeholder = "0" };
            amountEntryCell.Entry.Keyboard = Keyboard.Numeric;
            currencyCell = new CurrencyEntryCell(navigation) { IsAmountEnabled = false, IsFormRepresentation = true };
            inputSection.Add(amountEntryCell);
            inputSection.Add(currencyCell);

        }

        public override TableSection InputSection => inputSection;
        public override string DefaultName => I18N.LocalAccount;
        public override string Description => I18N.Manually;

        public override bool Enabled
        {
            set
            {
                currencyCell.IsEditable = value;
            }
        }

        public override FunctionalAccount GetAccount(string name)
        {
            var currency = currencyCell.SelectedCurrency;
            var amount = decimal.Parse(string.IsNullOrEmpty(amountEntryCell.Text) ? "0" : amountEntryCell.Text);

            return currency == null ? null : new LocalAccount(null, name, new Money(amount, currency), AccountStorage.Instance.LocalRepository.Id);
        }

        public override void Unfocus()
        {
            currencyCell.Unfocus();
        }
    }
}
