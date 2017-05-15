using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Ui.Helpers;

namespace MyCC.Ui.DataItems
{
    public class CoinInfoItem
    {
        private readonly CoinInfoData _data;
        private readonly Currency _currency;

        public CoinInfoItem(CoinInfoData coinInfoData, string explorer, Currency currency)
        {
            _data = coinInfoData ?? new CoinInfoData(currency);
            Explorer = explorer;
            _pow = StringHelper.TextResolver.CoinProofOfWork;
            _pos = StringHelper.TextResolver.CoinProofOfStake;
            _currency = currency;
        }

        public readonly string Explorer;

        private readonly string _pow, _pos;

        public string Algorithm => _data.Algorithm;

        public string Type => (_data.IsProofOfWork ?? false) && (_data.IsProofOfStake ?? false) ? $"{_pos}, {_pow}" :
            _data.IsProofOfWork.GetValueOrDefault() ? _pow :
            _data.IsProofOfStake.GetValueOrDefault() ? _pos :
            string.Empty;

        public string Hashrate => $"{_data.Hashrate ?? 0:#,0.########} {StringHelper.TextResolver.GHps}";
        public string Difficulty => $"{_data.Difficulty ?? 0:#,0.########}";

        public string Blockreward => new Money(_data.Blockreward ?? 0, _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string Blockheight => $"{_data.BlockHeight ?? 0:#,0}";
        public string Blocktime => $"{_data.Blocktime ?? 0:#,0.##} {StringHelper.TextResolver.UnitSecond}";

        public string Supply => new Money(_data.CoinSupply ?? 0, _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string MaxSupply => new Money(_data.MaxCoinSupply ?? 0, _currency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
        public string MarketCap => new Money((_data.CoinSupply ?? 0) * (ExchangeRateHelper.GetRate(_currency, CurrencyConstants.Btc)?.Rate ?? 0), CurrencyConstants.Btc).ToStringTwoDigits(ApplicationSettings.RoundMoney);


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
        public bool HasMarketCap => HasSupply && ExchangeRateHelper.GetRate(_currency, CurrencyConstants.Btc)?.Rate != null;
    }
}