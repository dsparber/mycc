using System;
using System.Linq;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.ModelExtensions
{
    public static class ExchangeRateExtensionMethodes
    {
        public static ExchangeRate Inverse(this ExchangeRate exchangeRate)
        {
            var inverseRate = 1 / exchangeRate.Rate;
            var inverseDescriptor = exchangeRate.RateDescriptor.Inverse();

            return new ExchangeRate(inverseDescriptor, inverseRate, exchangeRate.SourceId)
            {
                LastUpdate = exchangeRate.LastUpdate
            };
        }

        public static ExchangeRate CombineWith(this ExchangeRate referenceRate, ExchangeRate secondaryRate)
        {
            if (referenceRate == null || secondaryRate == null) return null;

            var referenceDescriptor = referenceRate.RateDescriptor;
            var secondaryDescriptor = secondaryRate.RateDescriptor;
            var combinedRateDescriptor = new RateDescriptor(referenceDescriptor.FindDifferentCurrencyTo(secondaryDescriptor), secondaryDescriptor.FindDifferentCurrencyTo(referenceDescriptor));

            var commonCurrencyId = referenceDescriptor.FindCommonCurrencyIdWith(secondaryDescriptor);
            var rate = secondaryRate.InvertIfNeeded(commonCurrencyId).Rate / referenceRate.InvertIfNeeded(commonCurrencyId).Rate;

            return new ExchangeRate(combinedRateDescriptor, rate, lastUpate: new[] { referenceRate.LastUpdate, secondaryRate.LastUpdate }.Min());
        }


        private static ExchangeRate InvertIfNeeded(this ExchangeRate rate, string currencyId)
        {
            return currencyId.Equals(rate.RateDescriptor.ReferenceCurrencyId) ? rate : rate.Inverse();
        }
    }
}