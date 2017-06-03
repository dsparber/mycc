using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories
{
    public abstract class MultiUriJsonRateSource : IRateSource
    {
        public abstract RateSourceId Id { get; }
        public abstract string Name { get; }
        public abstract RateSourceType Type { get; }

        protected abstract Uri GetUri(RateDescriptor rateDescriptor);

        protected (string name, string value)? HttpHeader = null;

        public abstract bool IsAvailable(RateDescriptor rate);

        public async Task<IEnumerable<ExchangeRate>> FetchRates(IEnumerable<RateDescriptor> rateDescriptors)
        {
            var descriptorList = rateDescriptors.ToList();
            if (!descriptorList.Any(IsAvailable)) return new List<ExchangeRate>();

            try
            {
                var rates = new List<ExchangeRate>();
                foreach (var rateDescriptor in descriptorList)
                {
                    var rate = await GetRate(rateDescriptor);
                    if (rate != null) rates.Add(rate);
                }
                return rates;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        private async Task<ExchangeRate> GetRate(RateDescriptor rateDescriptor)
        {
            var uri = GetUri(rateDescriptor);
            if (uri == null) return null;
            var json = await uri.GetJson(HttpHeader);
            var rate = GetRateFromJson(json);
            return rate == null ? null : new ExchangeRate(rateDescriptor, rate.Value, (int)Id, DateTime.Now);
        }

        protected abstract decimal? GetRateFromJson(JToken json);
    }
}

