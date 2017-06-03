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

        public static ExchangeRate CombineWith(this ExchangeRate rate1, ExchangeRate rate2)
        {
            if (rate1 == null || rate2 == null) return null;

            var r = new ExchangeRate(DifferentCurrency(rate1, rate2).Id, DifferentCurrency(rate2, rate1).Id, new[] { rate1.LastUpdate, rate2.LastUpdate }.Min());

            var r1 = InvertIfNeeded(rate1, CommonCurrency(rate1, rate2));
            var r2 = InvertIfNeeded(rate2, CommonCurrency(rate2, rate1));

            if (r1.Rate != null && r1.Rate != 0) // To avoid divided by 0
            {
                r.Rate = r2.Rate / r1.Rate;
            }

            return r;
        }

        private static Currestringncy FindDifferentCurrencyId(ExchangeRate r1, ExchangeRate r2)
        {
            if (GetCommonCurrencyId(r1, r2) == null) throw new InvalidOperationException("No common currency");

            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ?
                new Currency(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) :
                new Currency(r1.ReferenceCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency);
        }

        private static string GetCommonCurrencyId(ExchangeRate r1, ExchangeRate r2)
        {
            return r2.Contains(r1.ReferenceCurrencyCode, r1.ReferenceCurrencyIsCryptoCurrency) ? r1.ReferenceCurrencyCode : r2.Contains(r1.SecondaryCurrencyCode, r1.SecondaryCurrencyIsCryptoCurrency) ? r1.SecondaryCurrencyCode : null;
        }

        private static ExchangeRate InvertIfNeeded(ExchangeRate rate, string currencyCode)
        {
            return currencyCode.Equals(rate.ReferenceCurrencyCode) ? rate : rate.Inverse;
        }
    }
}