using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Ui.DataItems
{
    public class CoinHeaderItem : HeaderItem
    {
        public CoinHeaderItem(Money referenceMoney, IEnumerable<Money> additionalReferences) : base(null, null)
        {
            var additionalReferencesOrdered = additionalReferences.OrderBy(m => m.Currency.Code).ToList();

            MainText = referenceMoney.MaxTwoDigits();
            InfoText = additionalReferencesOrdered.Any() ? string.Join(" / ", additionalReferencesOrdered.Select(m => m.MaxTwoDigits())) : referenceMoney.Currency.Name;
        }
    }
}