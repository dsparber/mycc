using System;
using MyCC.Core.Rates.Models.Extensions;

namespace MyCC.Core.Rates.Models
{

    public class ExchangeRate
    {
        private readonly decimal _rate;
        private readonly DateTime _lastUpdate;

        internal readonly int SourceId;
        internal readonly string Id;

        public decimal Rate => Descriptor.HasEqualCurrencies() ? 1 : _rate;
        public DateTime LastUpdate => Descriptor.HasEqualCurrencies() ? DateTime.Now : _lastUpdate;
        public readonly RateDescriptor Descriptor;

        public ExchangeRate(RateDescriptor descriptor, decimal rate, int sourceId = 0, DateTime? lastUpate = null)
        {
            if (rate <= 0) throw new ArgumentException("The rate needs to be greater than 0!");

            SourceId = sourceId;
            Descriptor = descriptor;

            Id = $"{descriptor.Id}_{sourceId}";
            _rate = rate;
            _lastUpdate = lastUpate ?? default(DateTime);
        }


        public override bool Equals(object obj) => Id.Equals((obj as ExchangeRate)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => $"1 {Descriptor.ReferenceCurrencyId} = {Rate} {Descriptor.SecondaryCurrencyId}";
    }
}