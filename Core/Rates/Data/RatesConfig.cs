using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Implementations;
using MyCC.Core.Settings;

namespace MyCC.Core.Rates.Data
{
    internal static class RatesConfig
    {
        public static readonly string DefaultFiatCurrencyId = CurrencyConstants.Eur.Id;
        public static readonly string DefaultCryptoCurrencyId = CurrencyConstants.Btc.Id;
        public static readonly RateDescriptor DefaultCryptoToFiatDescriptor = new RateDescriptor(DefaultFiatCurrencyId, DefaultCryptoCurrencyId);

        public static readonly IEnumerable<IRateSource> Sources = new IRateSource[] {
            new BittrexExchangeRateSource(),
            new BtceExchangeRateSource(),
            new CryptonatorExchangeRateSource(),
            new FixerIoExchangeRateSource(),
            new BitstampExchangeRateSource(),
            new KrakenExchangeRateSource(),
            new QuadrigaCxExchangeRateSource(),
            new CoinbaseExchangeRateSource(),
            new BitPayExchangeRateSource(),
            new BitfinexExchangeRateSource(),
            new CoinapultExchangeRateSource(),
            new ItBitExchangeRateSource()
        };

        public static IRateSource SelectedCryptoToFiatSource =>
            RatesConfig.Sources.FirstOrDefault(source => (int)source.Id == SelectedCryptoToFiatSourceId);

        public static int SelectedCryptoToFiatSourceId => ApplicationSettings.PreferredBitcoinRepository;
    }

}