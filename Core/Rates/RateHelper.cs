using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Settings;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates
{
    public static class RateHelper
    {
        internal static IEnumerable<RateDescriptor> NeededRates => GetNeededRates();
        private static IEnumerable<RateDescriptor> GetNeededRates()
        {
            var referenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToList();
            var usedCurrencies = AccountStorage.UsedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(ApplicationSettings.WatchedCurrencies);

            return usedCurrencies.SelectMany(currencyId => referenceCurrencies.Select(referenceCurrencyId => new RateDescriptor(currencyId, referenceCurrencyId))).Distinct();
        }


        public static ExchangeRate GetRate(this RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new ExchangeRate(rateDescriptor, 1);
            if (rateDescriptor.HasCryptoAndFiatCurrency()) return rateDescriptor.GetMixedRate();
            return rateDescriptor.GetPureRate();
        }

        private static ExchangeRate GetPureRate(this RateDescriptor rateDescriptor)
        {
            var useCrypto = rateDescriptor.HasOnlyCryptoCurrencies();
            var rateReferenceCurrency = rateDescriptor.ReferenceCurrencyId.RateToDefaultCurrency(useCrypto);
            var rateSecondaryCurrency = rateDescriptor.SecondaryCurrencyId.RateToDefaultCurrency(useCrypto);

            return rateReferenceCurrency.CombineWith(rateSecondaryCurrency);
        }

        private static ExchangeRate GetMixedRate(this RateDescriptor rateDescriptor)
        {
            if (RateStorage.SelectedCryptoToFiatSource.IsAvailable(rateDescriptor)) return RateStorage.GetRateOrDefault(rateDescriptor);

            var rateToDefaultFiatCurrency = rateDescriptor.GetFiatCurrencyId().RateToDefaultCurrency(useCrypto: false);
            var rateToDefaultCryptoCurrency = rateDescriptor.GetCryptoCurrencyId().RateToDefaultCurrency(useCrypto: true);
            var rateCryptoToFiat = RateStorage.GetRateOrDefault(RateConstants.DefaultCryptoToFiatDescriptor);

            return rateToDefaultFiatCurrency.CombineWith(rateCryptoToFiat).CombineWith(rateToDefaultCryptoCurrency);
        }

        private static ExchangeRate RateToDefaultCurrency(this string currencyId, bool useCrypto)
        {
            var defaultCurrencyId = useCrypto ? RateConstants.DefaultCryptoCurrencyId : RateConstants.DefaultFiatCurrencyId;
            var rateDescriptor = new RateDescriptor(currencyId, defaultCurrencyId);
            return RateStorage.GetRateOrDefault(rateDescriptor);
        }


        internal static IEnumerable<RateDescriptor> GetNeededRatesForCalculation(RateDescriptor rateDescriptor)
        {
            if (rateDescriptor.HasEqualCurrencies()) return new List<RateDescriptor>();
            if (rateDescriptor.HasCryptoAndFiatCurrency()) return rateDescriptor.GetNeededForMixedRate();
            return rateDescriptor.GetNeededForPureRate();
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
            if (RateStorage.SelectedCryptoToFiatSource.IsAvailable(rateDescriptor)) return new[] { rateDescriptor };

            return new[]
            {
                rateDescriptor.GetFiatCurrencyId().GetDescriptorToDefaultCurrency(useCrypto: false),
                rateDescriptor.GetCryptoCurrencyId().GetDescriptorToDefaultCurrency(useCrypto: true),
                RateConstants.DefaultCryptoToFiatDescriptor
            }.Distinct().Where(descriptor => !descriptor.HasEqualCurrencies());
        }

        private static RateDescriptor GetDescriptorToDefaultCurrency(this string currencyId, bool useCrypto)
        {
            var defaultCurrencyId = useCrypto ? RateConstants.DefaultCryptoCurrencyId : RateConstants.DefaultFiatCurrencyId;
            return new RateDescriptor(currencyId, defaultCurrencyId);
        }

    }
}
