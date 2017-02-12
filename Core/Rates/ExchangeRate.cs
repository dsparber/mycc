using System;
using MyCC.Core.Account.Models.Base;
using SQLite;

namespace MyCC.Core.Rates
{

    /// <summary>
    /// Simple exchange rate model
    /// </summary>
    [Table("ExchangeRates")]
    public class ExchangeRate
    {

        public ExchangeRate() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class.
        /// </summary>
        /// <param name="referenceCurrencyCode">Reference currency code.</param>
        /// <param name="referenceIsCrypto">Wether the reference currency is a crypto currency</param>
        /// <param name="secondaryCurrencyCode">Secondary currency code.</param>
        /// <param name="secondaryIsCrypto">Wether the secondary currency is a crypto currency</param>
        /// <param name="lastUpdate">Last update of this exchange rate</param>
        /// <param name="rate">Exchange rate.</param>
        public ExchangeRate(string referenceCurrencyCode, bool referenceIsCrypto, string secondaryCurrencyCode, bool secondaryIsCrypto, DateTime? lastUpdate = null, decimal? rate = null)
        {
            if (string.IsNullOrWhiteSpace(referenceCurrencyCode) || string.IsNullOrWhiteSpace(secondaryCurrencyCode))
            {
                throw new ArgumentNullException();
            }
            ReferenceCurrencyCode = referenceCurrencyCode.ToUpper();
            SecondaryCurrencyCode = secondaryCurrencyCode.ToUpper();
            ReferenceCurrencyIsCryptoCurrency = referenceIsCrypto;
            SecondaryCurrencyIsCryptoCurrency = secondaryIsCrypto;
            Rate = rate == null ? (decimal?)null : Math.Truncate(rate.Value * 100000000) / 100000000;
            LastUpdate = lastUpdate ?? DateTime.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class.
        /// </summary>
        /// <param name="referenceCurrency">Reference currency.</param>
        /// <param name="secondaryCurrency">Secondary currency.</param>
        /// <param name="lastUpdate">Last update of this exchange rate</param>
        /// <param name="rate">Exchange rate.</param>
        public ExchangeRate(Currency.Model.Currency referenceCurrency, Currency.Model.Currency secondaryCurrency, DateTime? lastUpdate = null, decimal? rate = null) : this(referenceCurrency.Code, referenceCurrency.IsCryptoCurrency, secondaryCurrency.Code, secondaryCurrency.IsCryptoCurrency, lastUpdate, rate) { }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey, Column("_id")]
        public string Id
        {
            get
            {
                return $"{ReferenceCurrencyCode}-{SecondaryCurrencyCode}-{RepositoryId}";
            }
            set
            {
                var parts = value.Split('-');
                ReferenceCurrencyCode = parts[0];
                SecondaryCurrencyCode = parts[1];
                RepositoryId = int.Parse(parts[2]);
            }
        }

        /// <summary>
        /// Gets or sets the repository identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Column("ExchangeRateRepository")]
        public int RepositoryId { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// The reference currency.
        /// </summary>
        [Column("ReferenceCode")]
        public string ReferenceCurrencyCode { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MyCC.Core.Rates.ExchangeRate"/> reference currency is a crypto currency.
        /// </summary>
        /// <value><c>true</c> if reference currency is crypto currency; otherwise, <c>false</c>.</value>
        [Column("ReferenceIsCrypto")]
        public bool ReferenceCurrencyIsCryptoCurrency { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// The secondary currency.
        /// </summary>
        [Column("SecondaryCode")]
        public string SecondaryCurrencyCode { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MyCC.Core.Rates.ExchangeRate"/> secondary currency is a crypto currency.
        /// </summary>
        /// <value><c>true</c> if secondary currency is crypto currency; otherwise, <c>false</c>.</value>
        [Column("SecondaryIsCrypto")]
        public bool SecondaryCurrencyIsCryptoCurrency { get; set; }

        [Column("LastUpdate")]
        public long LastUpdateTicks { get; set; }

        [Ignore]
        public DateTime LastUpdate
        {
            get { return new DateTime(LastUpdateTicks); }
            set { LastUpdateTicks = value.Ticks; }
        }

        [Ignore]
        public Money AsMoney => new Money(Rate ?? 0, new Currency.Model.Currency(SecondaryCurrencyCode, SecondaryCurrencyIsCryptoCurrency));

        private decimal? _rate;
        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        /// <value>The exchange rate</value>
        [Column("Rate")]
        public decimal? Rate
        {
            get { return _rate; }
            set
            {
                if (value != null && value < 0)
                {
                    throw new ArgumentException("The rate can not be negative!");
                }
                _rate = value;
            }
        }


        /// <summary>
        /// Gets the inverse exchange rate.
        /// </summary>
        /// <returns>The inverse exchange rate.</returns>
        [Ignore]
        public ExchangeRate Inverse
        {
            get
            {
                var exchangeRate = new ExchangeRate(SecondaryCurrencyCode, SecondaryCurrencyIsCryptoCurrency, ReferenceCurrencyCode, ReferenceCurrencyIsCryptoCurrency, LastUpdate) { RepositoryId = RepositoryId };
                if (Rate != null && Rate != 0)
                {
                    exchangeRate.Rate = 1 / Rate;
                }
                return exchangeRate;
            }
        }

        /// <summary>
        /// Returns if the exchange rate contains the specified currency.
        /// </summary>
        /// <param name="currencyCode">The specified currency.</param>
        /// <param name="isCrypto">Wether the currency is a crypro currency</param>
        public bool Contains(string currencyCode, bool isCrypto)
        {
            return (ReferenceCurrencyCode != null && ReferenceCurrencyCode.Equals(currencyCode) && ReferenceCurrencyIsCryptoCurrency == isCrypto)
                || (SecondaryCurrencyCode != null && SecondaryCurrencyCode.Equals(currencyCode) && SecondaryCurrencyIsCryptoCurrency == isCrypto);
        }

        /// <summary>
        /// Checks if the instance equals the specified object.
        /// </summary>
        /// <param name="obj">The comparison object.</param>
        public override bool Equals(object obj)
        {
            var e = obj as ExchangeRate;
            return e != null &&
                (e.ReferenceCurrencyCode?.Equals(ReferenceCurrencyCode) ?? false) &&
                (e.SecondaryCurrencyCode?.Equals(SecondaryCurrencyCode) ?? false) &&
                e.ReferenceCurrencyIsCryptoCurrency == ReferenceCurrencyIsCryptoCurrency &&
                e.SecondaryCurrencyIsCryptoCurrency == SecondaryCurrencyIsCryptoCurrency;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return ReferenceCurrencyCode?.GetHashCode() ?? 0 + SecondaryCurrencyCode?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Get the exchange rate as String.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            if (Rate != null && Rate.Value > 0)
            {
                return $"1 {ReferenceCurrencyCode} = {Rate} {SecondaryCurrencyCode}";
            }
            return $"{ReferenceCurrencyCode} <-> {SecondaryCurrencyCode}";
        }
    }
}