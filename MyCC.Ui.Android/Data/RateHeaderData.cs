﻿using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Settings;

namespace MyCC.Ui.Android.Data
{
    public class RateHeaderData
    {
        public string MainText => _referenceMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string InfoText => string.Join(" / ", _additionalReferences.Select(m => m.ToStringTwoDigits(ApplicationSettings.RoundMoney)));

        private readonly Money _referenceMoney;
        private readonly List<Money> _additionalReferences;

        public RateHeaderData(Money referenceMoney, List<Money> additionalReferences)
        {
            _referenceMoney = referenceMoney;
            _additionalReferences = additionalReferences;
        }
    }
}