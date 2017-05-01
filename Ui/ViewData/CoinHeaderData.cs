using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.ViewData
{
    public class CoinHeaderData : HeaderDataItem
    {
        public override string MainText => _referenceMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public override string InfoText => string.Join(" / ", _additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney)));

        private readonly Money _referenceMoney;
        private readonly List<Money> _additionalReferences;

        public CoinHeaderData(Money referenceMoney, List<Money> additionalReferences) : base(null, null)
        {
            _referenceMoney = referenceMoney;
            _additionalReferences = additionalReferences.OrderBy(m => m.Currency.Code).ToList();
        }
    }
}