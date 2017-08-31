using System.Linq;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.ModelExtensions
{
    public static class ExchangeRateExtensionMethodes
    {
        public static ExchangeRate Inverse(this ExchangeRate exchangeRate)
        {
            var inverseRate = exchangeRate.Rate != 0 ? 1 / exchangeRate.Rate : 0;
            var inverseDescriptor = exchangeRate.Descriptor.Inverse();

            return new ExchangeRate(inverseDescriptor, inverseRate, exchangeRate.SourceId, exchangeRate.LastUpdate);
        }

        public static ExchangeRate CombineWith(this ExchangeRate referenceRate, ExchangeRate secondaryRate)
        {
            if (referenceRate == null || secondaryRate == null) return null;

            var referenceDescriptor = referenceRate.Descriptor;
            var secondaryDescriptor = secondaryRate.Descriptor;

            if (referenceDescriptor.HasEqualCurrencies()) return secondaryRate;
            if (secondaryDescriptor.HasEqualCurrencies()) return referenceRate;

            var combinedRateDescriptor = new RateDescriptor(referenceDescriptor.FindDifferentCurrencyTo(secondaryDescriptor), secondaryDescriptor.FindDifferentCurrencyTo(referenceDescriptor));

            var commonCurrencyId = referenceDescriptor.FindCommonCurrencyIdWith(secondaryDescriptor);
            var rate = secondaryRate.InvertIfNeeded(commonCurrencyId).Rate / referenceRate.InvertIfNeeded(commonCurrencyId).Rate;

            return new ExchangeRate(combinedRateDescriptor, rate, lastUpate: new[] { referenceRate.LastUpdate, secondaryRate.LastUpdate }.Min());
        }


        private static ExchangeRate InvertIfNeeded(this ExchangeRate rate, string currencyId)
        {
            return currencyId.Equals(rate.Descriptor.ReferenceCurrencyId) ? rate : rate.Inverse();
        }
    }
}