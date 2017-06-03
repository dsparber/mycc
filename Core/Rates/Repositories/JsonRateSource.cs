using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories
{
    public abstract class JsonRateSource : IRateSource
    {
        protected abstract Uri Uri { get; }
        public abstract RateSourceId Id { get; }
        public abstract string Name { get; }
        public abstract RateSourceType Type { get; }

        public abstract bool IsAvailable(RateDescriptor rate);

        public async Task<IEnumerable<ExchangeRate>> FetchRates(IEnumerable<RateDescriptor> rateDescriptors)
        {
            if (!rateDescriptors.Any(IsAvailable)) return new List<ExchangeRate>();

            try
            {
                var json = await Uri.GetJson();
                return GetRatesFromJson(json) ?? new List<ExchangeRate>();
            }
            catch (Exception e)
            {
                e.LogError();
                return new List<ExchangeRate>();
            }
        }

        protected abstract IEnumerable<ExchangeRate> GetRatesFromJson(JToken json);
    }
}