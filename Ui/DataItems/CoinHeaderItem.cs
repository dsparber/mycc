using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.DataItems
{
    public class CoinHeaderItem : HeaderItem
    {
        public override string MainText => _referenceMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public override string InfoText => _additionalReferences.Any() ? string.Join(" / ", _additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : _referenceMoney.Currency.Name;

        private readonly Money _referenceMoney;
        private readonly List<Money> _additionalReferences;

        public CoinHeaderItem(Money referenceMoney, IEnumerable<Money> additionalReferences) : base(null, null)
        {
            _referenceMoney = referenceMoney;
            _additionalReferences = additionalReferences.OrderBy(m => m.Currency.Code).ToList();
        }
    }
}