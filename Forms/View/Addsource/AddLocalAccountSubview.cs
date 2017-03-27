using System;
using System.Collections.Generic;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Addsource
{
	public class AddLocalAccountSubview : AddAccountSubview
	{
		private readonly TableSection _inputSection;
		private readonly CurrencyEntryCell _currencyCell;
		private readonly CustomEntryCell _amountEntryCell;

		public AddLocalAccountSubview(INavigation navigation)
		{
			_inputSection = new TableSection { Title = I18N.Details };
			_amountEntryCell = new CustomEntryCell { Title = I18N.Amount, Placeholder = "0", IsNumeric = true };
			_currencyCell = new CurrencyEntryCell(navigation, null) { IsAmountEnabled = false, IsFormRepresentation = true };
			_inputSection.Add(_amountEntryCell);
			_inputSection.Add(_currencyCell);

		}

		public override List<TableSection> InputSections => new List<TableSection> { _inputSection };
		public override string Description => I18N.Manually;

		public override bool Enabled
		{
			set
			{
				_currencyCell.IsEditable = value;
			}
		}

		public override FunctionalAccount GetAccount(string name)
		{
			try
			{
				var currency = _currencyCell.SelectedCurrency;
				var amount = decimal.Parse(string.IsNullOrEmpty(_amountEntryCell.Text) ? "0" : _amountEntryCell.Text);
				return currency == null ? null : new LocalAccount(null, name, new Money(amount, currency), true, DateTime.Now, AccountStorage.Instance.LocalRepository.Id);
			}
			catch
			{
				return null;
			}

		}

		public override void Unfocus()
		{
			_currencyCell.Unfocus();
		}
	}
}
