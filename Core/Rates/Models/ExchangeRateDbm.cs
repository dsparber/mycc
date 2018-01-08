using System;
using System.Linq;
using MyCC.Core.Currencies.Models;
using Newtonsoft.Json;
using SQLite;

// ReSharper disable MemberCanBePrivate.Global

namespace MyCC.Core.Rates.Models
{

    /// <summary>
    /// Simple exchange rate model
    /// </summary>
    [Table("ExchangeRates")]
    internal class ExchangeRateDbm : IEquatable<ExchangeRateDbm> 
    {
        private string _id;

        [PrimaryKey, Column("Id")]
        public string Id
        {
            get => _id;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                if (_id != null) throw new InvalidOperationException("Id can not be changed after first assignment");

                _id = value;
            }
        }

        [Column("SourceId")]
        public int SourceId { get; set; }

        [Column("ReferenceCurrencyId")]
        public string ReferenceCurrencyId { get; set; }

        [Column("SecondaryeCurrencyId")]
        public string SecondaryCurrencyId { get; set; }

        [Column("Rate")]
        public decimal Rate { get; set; }

        [Column("LastUpdate")]
        public DateTime LastUpdate { get; set; }


        public ExchangeRateDbm() { }

        public ExchangeRateDbm(ExchangeRate exchangeRate)
        {
            var orderedCurrencyIds = new[] { exchangeRate.Descriptor.ReferenceCurrencyId, exchangeRate.Descriptor.SecondaryCurrencyId }.OrderBy(id => id).ToList();
            Id = $"{orderedCurrencyIds[0]}_{orderedCurrencyIds[1]}_{exchangeRate.SourceId}";
            ReferenceCurrencyId = exchangeRate.Descriptor.ReferenceCurrencyId;
            SecondaryCurrencyId = exchangeRate.Descriptor.SecondaryCurrencyId;
            Rate = exchangeRate.Rate;
            SourceId = exchangeRate.SourceId;
            LastUpdate = exchangeRate.LastUpdate;
        }

        [Ignore]
        [JsonIgnore]
        public ExchangeRate AsExchangeRate
        {
            get
            {
                var rateDescriptor = new RateDescriptor(ReferenceCurrencyId, SecondaryCurrencyId);
                return new ExchangeRate(rateDescriptor, Rate, SourceId, LastUpdate);
            }
        }


        public override bool Equals(object obj) => string.Equals(Id, (obj as ExchangeRateDbm)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;

        public bool Equals(ExchangeRateDbm other) => Id.Equals(other.Id);
    }
}