using System.Collections.Generic;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Ui.Helpers;

namespace MyCC.Ui.DataItems
{
    public class CoinInfoItem
    {
        private readonly CoinInfoData _data;
        private readonly string _currencyId;

        public CoinInfoItem(CoinInfoData coinInfoData, IEnumerable<string> explorer, string currencyId)
        {
            _data = coinInfoData ?? new CoinInfoData(currencyId);
            Explorer = string.Join(", ", explorer);
            _pow = StringUtils.TextResolver.CoinProofOfWork;
            _pos = StringUtils.TextResolver.CoinProofOfStake;
            _currencyId = currencyId;
        }

        public readonly string Explorer;

        private readonly string _pow, _pos;

        public string Algorithm => _data.Algorithm;

        public string Type => (_data.IsProofOfWork ?? false) && (_data.IsProofOfStake ?? false) ? $"{_pos}, {_pow}" :
            _data.IsProofOfWork.GetValueOrDefault() ? _pow :
            _data.IsProofOfStake.GetValueOrDefault() ? _pos :
            string.Empty;

        public string Hashrate => $"{(_data.Hashrate ?? 0).ToMax8DigitString()} {StringUtils.TextResolver.GHps}";
        public string Difficulty => (_data.Difficulty ?? 0).ToMax8DigitString();

        public string Blockreward => new Money(_data.Blockreward ?? 0, _currencyId.Find()).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string Blockheight => $"{_data.BlockHeight ?? 0:#,0}";
        public string Blocktime => $"{_data.Blocktime ?? 0:#,0.##} {StringUtils.TextResolver.UnitSecond}";

        public string Supply => new Money(_data.CoinSupply ?? 0, _currencyId.Find()).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string MaxSupply => new Money(_data.MaxCoinSupply ?? 0, _currencyId.Find()).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string MarketCap => new Money((_data.CoinSupply ?? 0) * (MyccUtil.Rates.GetRate(new RateDescriptor(_currencyId, CurrencyConstants.Btc.Id))?.Rate ?? 0), CurrencyConstants.Btc).ToStringTwoDigits(ApplicationSettings.RoundMoney);


        public bool HasExplorer => !string.IsNullOrWhiteSpace(Explorer);

        public bool HasAlgorithm => _data.Algorithm != null;
        public bool HasType => _data.IsProofOfStake != null || _data.IsProofOfWork != null;
        public bool HasDifficulty => _data.Difficulty != null;
        public bool HasHashrate => _data.Hashrate != null;

        public bool HasBlockreward => _data.Blockreward != null;
        public bool HasBlocktime => _data.Blocktime != null;
        public bool HasBlockheight => _data.BlockHeight != null;

        public bool HasSupply => _data.CoinSupply != null;
        public bool HasMaxSupply => _data.MaxCoinSupply != null;
        public bool HasMarketCap => HasSupply && MyccUtil.Rates.HasRate(new RateDescriptor(_currencyId, CurrencyConstants.Btc.Id));
    }
}