using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Models.Extensions;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Sources.Utils
{
    internal abstract class MultiUriJsonRateSource : IRateSource
    {
        public abstract int Id { get; }
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
            try
            {
                var json = await uri.GetJson(HttpHeader);
                var result = GetRateFromJson(json, rateDescriptor);
                return result.rate == null ? null : new ExchangeRate(result.inverse ? rateDescriptor.Inverse() : rateDescriptor, result.rate.Value, Id, DateTime.Now);
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        protected abstract (decimal? rate, bool inverse) GetRateFromJson(JToken json, RateDescriptor rateDescriptor);
    }
}

