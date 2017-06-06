using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.Utils
{
    public static class DistinctDescriptorsUtil
    {
        public static IEnumerable<RateDescriptor> DistinctCurrencyPairs(this IEnumerable<RateDescriptor> source)
        {
            var result = new List<RateDescriptor>();
            foreach (var descriptor in source)
            {
                if (!result.Any(existing => existing.CurrenciesEqual(descriptor)))
                {
                    result.Add(descriptor);
                }
            }
            return result;
        }

        public static IEnumerable<ExchangeRate> DistinctCurrencyPairs(this IEnumerable<ExchangeRate> source)
        {
            var result = new List<ExchangeRate>();
            foreach (var rate in source.Distinct())
            {
                if (!result.Any(existing => existing.Descriptor.CurrenciesEqual(rate.Descriptor)))
                {
                    result.Add(rate);
                }
            }
            return result;
        }

        public static bool Contains(this IEnumerable<ExchangeRate> source, RateDescriptor descriptor)
        {
            return source.Any(rate => rate.Descriptor.CurrenciesEqual(descriptor));
        }
    }
}