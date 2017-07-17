using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;

namespace MyCC.Core.Rates.Utils
{
    internal static class CryptoToFiatInfoUtils
    {
        public static string GetDetailText(this IReadOnlyCollection<ExchangeRate> rates)
        {
            var descriptorBtcUsd = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id);
            var descriptorBtcEur = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id);

            var usd = rates.FirstOrDefault(rate => rate.Descriptor.CurrenciesEqual(descriptorBtcUsd));
            var eur = rates.FirstOrDefault(rate => rate.Descriptor.CurrenciesEqual(descriptorBtcEur));

            usd = usd?.Descriptor.CurrenciesEqual(descriptorBtcUsd) ?? false ? usd : usd?.Inverse();
            eur = eur?.Descriptor.CurrenciesEqual(descriptorBtcEur) ?? false ? eur : eur?.Inverse();

            var usdString = new Money(usd?.Rate ?? 0, CurrencyConstants.Usd).TwoDigits();
            var eurString = new Money(eur?.Rate ?? (usd != null ? MyccUtil.Rates.GetRate(descriptorBtcEur)?.Rate : 0) ?? 0, CurrencyConstants.Eur).TwoDigits();
            var note = eur == null && usd != null ? "*" : string.Empty;

            return $"{eurString}{note} / {usdString}";
        }

        public static IEnumerable<(string name, IEnumerable<ExchangeRate> rates)> CryptoToFiatSourcesWithRates
            => RatesConfig.Sources.Where(source => source.Type == RateSourceType.CryptoToFiat).Select(source =>
            {
                var rates = RateDatabase.CryptoToFiatRates.Where(rate => rate.SourceId == source.Id);
                return (source.Name, rates);
            });
    }
}