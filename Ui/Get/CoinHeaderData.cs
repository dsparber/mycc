using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Get
{
    public class CoinHeaderData : HeaderDataItem
    {
        public override string MainText => _referenceMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public override string InfoText => _additionalReferences.Any() ? string.Join(" / ", _additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney))) : _referenceMoney.Currency.Name;

        private readonly Money _referenceMoney;
        private readonly List<Money> _additionalReferences;

        public CoinHeaderData(Money referenceMoney, IEnumerable<Money> additionalReferences) : base(null, null)
        {
            _referenceMoney = referenceMoney;
            _additionalReferences = additionalReferences.OrderBy(m => m.Currency.Code).ToList();
        }
    }
}