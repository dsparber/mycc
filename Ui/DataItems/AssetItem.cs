using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.DataItems
{
    public class AssetItem
    {
        public readonly string CurrencyCode;
        public readonly string FormattedValue;
        public readonly string FormattedReferenceValue;
        public readonly bool Enabled;

        public readonly string CurrencyId;
        public readonly decimal Amount;
        public readonly decimal ReferenceAmount;

        public AssetItem(Money value, Money referenceValue, bool enabled)
        {
            CurrencyCode = value.Currency.Code;
            FormattedValue = value.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
            FormattedReferenceValue = referenceValue.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
            Enabled = enabled;
            CurrencyId = value.Currency.Id;
            Amount = value.Amount;
            ReferenceAmount = referenceValue.Amount;
        }

    }
}