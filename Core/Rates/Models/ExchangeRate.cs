using System;
using MyCC.Core.Rates.ModelExtensions;

namespace MyCC.Core.Rates.Models
{

    public class ExchangeRate
    {
        private readonly decimal _rate;
        private readonly DateTime _lastUpdate;

        internal readonly int SourceId;
        internal readonly string Id;

        public decimal Rate => RateDescriptor.HasEqualCurrencies() ? 1 : _rate;
        public DateTime LastUpdate => RateDescriptor.HasEqualCurrencies() ? DateTime.Now : _lastUpdate;
        public readonly RateDescriptor RateDescriptor;

        public ExchangeRate(RateDescriptor rateDescriptor, decimal rate, int sourceId = 0, DateTime? lastUpate = null)
        {
            if (rate <= 0) throw new ArgumentException("The rate needs to be greater than 0!");

            SourceId = sourceId;
            RateDescriptor = rateDescriptor;

            Id = $"{rateDescriptor.Id}_{sourceId}";
            _rate = Math.Round(rate, 8);
            _lastUpdate = lastUpate ?? default(DateTime);
        }


        public override bool Equals(object obj) => Id.Equals((obj as ExchangeRate)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => $"1 {RateDescriptor.ReferenceCurrencyId} = {Rate} {RateDescriptor.SecondaryCurrencyId}";
    }
}