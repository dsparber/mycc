using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.Utils
{
    internal static class RateCalculator
    {
        public static ExchangeRate GetRate(this RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new ExchangeRate(rateDescriptor, 1);
            return rateDescriptor.HasCryptoAndFiatCurrency()
                ? rateDescriptor.GetMixedRate()
                : rateDescriptor.GetPureRate();
        }

        private static ExchangeRate GetPureRate(this RateDescriptor rateDescriptor)
        {
            var useCrypto = rateDescriptor.HasOnlyCryptoCurrencies();
            var rateReferenceCurrency = rateDescriptor.ReferenceCurrencyId.RateToDefaultCurrency(useCrypto);
            var rateSecondaryCurrency = rateDescriptor.SecondaryCurrencyId.RateToDefaultCurrency(useCrypto);

            var rate = rateReferenceCurrency.CombineWith(rateSecondaryCurrency);
            if (rate == null) return null;
            return rate.Descriptor.Equals(rateDescriptor) ? rate : rate.Inverse();
        }

        private static ExchangeRate GetMixedRate(this RateDescriptor rateDescriptor)
        {
            if (RatesConfig.SelectedCryptoToFiatSource?.IsAvailable(rateDescriptor) ?? false)
            {
                var directRate = RateDatabase.GetRateOrDefault(rateDescriptor);
                if (directRate != null)
                    return directRate.Descriptor.Equals(rateDescriptor) ? directRate : directRate.Inverse();
            }

            var rateToDefaultFiatCurrency = rateDescriptor.GetFiatCurrencyId().RateToDefaultCurrency(useCrypto: false);
            var rateToDefaultCryptoCurrency = rateDescriptor.GetCryptoCurrencyId().RateToDefaultCurrency(useCrypto: true);
            var rateCryptoToFiat = RateDatabase.GetRateOrDefault(RatesConfig.DefaultCryptoToFiatDescriptor);

            var rate = rateToDefaultFiatCurrency.CombineWith(rateCryptoToFiat).CombineWith(rateToDefaultCryptoCurrency);
            if (rate == null) return null;
            return rate.Descriptor.Equals(rateDescriptor) ? rate : rate.Inverse();
        }

        private static ExchangeRate RateToDefaultCurrency(this string currencyId, bool useCrypto)
        {
            var defaultCurrencyId = useCrypto ? RatesConfig.DefaultCryptoCurrencyId : RatesConfig.DefaultFiatCurrencyId;
            var rateDescriptor = new RateDescriptor(currencyId, defaultCurrencyId);
            return RateDatabase.GetRateOrDefault(rateDescriptor);
        }


        public static IEnumerable<RateDescriptor> GetNeededRatesForCalculation(this RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new List<RateDescriptor>();
            return rateDescriptor.HasCryptoAndFiatCurrency()
                ? rateDescriptor.GetNeededForMixedRate()
                : rateDescriptor.GetNeededForPureRate();
        }

        private static IEnumerable<RateDescriptor> GetNeededForPureRate(this RateDescriptor rateDescriptor)
        {
            var useCrypto = rateDescriptor.HasOnlyCryptoCurrencies();
            return new[]
            {
                rateDescriptor.ReferenceCurrencyId.GetDescriptorToDefaultCurrency(useCrypto),
                rateDescriptor.SecondaryCurrencyId.GetDescriptorToDefaultCurrency(useCrypto)
            }.Distinct().Where(descriptor => !descriptor.HasEqualCurrencies());
        }

        private static IEnumerable<RateDescriptor> GetNeededForMixedRate(this RateDescriptor rateDescriptor)
        {
            if (RatesConfig.SelectedCryptoToFiatSource.IsAvailable(rateDescriptor)) return new[] { rateDescriptor };

            return new[]
            {
                rateDescriptor.GetFiatCurrencyId().GetDescriptorToDefaultCurrency(useCrypto: false),
                rateDescriptor.GetCryptoCurrencyId().GetDescriptorToDefaultCurrency(useCrypto: true),
                RatesConfig.DefaultCryptoToFiatDescriptor
            }.Distinct().Where(descriptor => !descriptor.HasEqualCurrencies());
        }

        private static RateDescriptor GetDescriptorToDefaultCurrency(this string currencyId, bool useCrypto)
        {
            var defaultCurrencyId = useCrypto ? RatesConfig.DefaultCryptoCurrencyId : RatesConfig.DefaultFiatCurrencyId;
            return new RateDescriptor(currencyId, defaultCurrencyId);
        }
    }
}