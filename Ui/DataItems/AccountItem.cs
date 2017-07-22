using MyCC.Core.Account.Models.Base;

namespace MyCC.Ui.DataItems
{
    public class AccountItem
    {
        public readonly string Name;
        public readonly string FormattedValue;
        public readonly int Id;
        public readonly decimal Amount;
        public readonly bool Enabled;

        public AccountItem(Account account)
        {
            FormattedValue = account.Money.EightDigits(false);
            Name = account.Name;
            Id = account.Id;
            Amount = account.Money.Amount;
            Enabled = account.IsEnabled;
        }
    }
}