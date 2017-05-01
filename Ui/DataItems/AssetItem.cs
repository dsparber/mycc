using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.DataItems
{
    public class AssetItem
    {
        public string CurrencyCode => Value.Currency.Code;
        public string FormattedValue => Value.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);

        public readonly bool Enabled;

        public string FormattedReferenceValue => ReferenceValue.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);

        public Money Value { get; private set; }

        public Money ReferenceValue { get; private set; }

        public AssetItem(Money value, Money referenceValue, bool enabled)
        {
            Value = value;
            ReferenceValue = referenceValue;
            Enabled = enabled;
        }

    }
}