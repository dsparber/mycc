using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.DataItems
{
    public class AccountItem
    {
        public readonly string Name;
        public readonly string FormattedValue;
        public readonly int Id;
        public readonly decimal Amount;

        public AccountItem(Account account)
        {
            FormattedValue = account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
            Name = account.Name;
            Id = account.Id;
            Amount = account.Money.Amount;
        }
    }
}