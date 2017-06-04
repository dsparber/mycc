using System;
using MyCC.Core.Rates.ModelExtensions;

namespace MyCC.Core.Rates.Models
{

    public class ExchangeRate
    {
        /// <summary>
        /// Generated Id based on currencies and source id
        /// </summary>
        public readonly string Id;

        public readonly int SourceId;

        public DateTime LastUpdate
        {
            get => RateDescriptor.HasEqualCurrencies() ? DateTime.Now : _lastUpdate;
            set => _lastUpdate = value;
        }

        public readonly RateDescriptor RateDescriptor;

        public decimal Rate
        {
            get => RateDescriptor.HasEqualCurrencies() ? 1 : _rate;
            set
            {
                if (value <= 0) throw new ArgumentException("The rate needs to be greater than 0!");
                _rate = Math.Round(value, 8);
            }
        }

        private decimal _rate;
        private DateTime _lastUpdate;


        public ExchangeRate() { }

        public ExchangeRate(RateDescriptor rateDescriptor, decimal rate, int sourceId = 0, DateTime? lastUpate = null)
        {
            RateDescriptor = rateDescriptor;
            Rate = rate;
            SourceId = sourceId;
            Id = $"{rateDescriptor.Id}_{SourceId}";
            LastUpdate = lastUpate ?? default(DateTime);
        }


        public override bool Equals(object obj) => Id.Equals((obj as ExchangeRate)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => $"1 {RateDescriptor.ReferenceCurrencyId} = {Rate} {RateDescriptor.SecondaryCurrencyId}";
    }
}