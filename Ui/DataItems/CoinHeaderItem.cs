using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.DataItems
{
    public class CoinHeaderItem : HeaderItem
    {
        public CoinHeaderItem(Money referenceMoney, IEnumerable<Money> additionalReferences) : base(null, null)
        {
            var additionalReferencesOrdered = additionalReferences.OrderBy(m => m.Currency.Code).ToList();

            MainText = referenceMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
            InfoText = additionalReferencesOrdered.Any() ? string.Join(" / ", additionalReferencesOrdered.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : referenceMoney.Currency.Name;
        }
    }
}