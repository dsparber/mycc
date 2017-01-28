using System;
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
        /// <param name="secondaryCurrencyCode">Secondary currency code.</param>
        /// <param name="rate">Exchange rate.</param>
        public ExchangeRate(string referenceCurrencyCode, bool referenceIsCrypto, string secondaryCurrencyCode, bool secondaryIsCrypto, decimal? rate = null)
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

            Id = $"{ReferenceCurrencyCode}{SecondaryCurrencyCode}{RepositoryId}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class.
        /// </summary>
        /// <param name="referenceCurrency">Reference currency.</param>
        /// <param name="secondaryCurrency">Secondary currency.</param>
        /// <param name="rate">Exchange rate.</param>
        public ExchangeRate(Currency.Model.Currency referenceCurrency, Currency.Model.Currency secondaryCurrency, decimal? rate = null) : this(referenceCurrency.Code, referenceCurrency.IsCryptoCurrency, secondaryCurrency.Code, secondaryCurrency.IsCryptoCurrency, rate) { }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey, Column("_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the repository identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Column("ExchangeRateRepository")]
        public int RepositoryId
        {
            get
            {
                return _repositoryId;
            }
            set
            {
                _repositoryId = value;
                Id = $"{ReferenceCurrencyCode}{SecondaryCurrencyCode}{RepositoryId}";
            }
        }

        private int _repositoryId;

        /// <summary>
        /// The reference currency.
        /// </summary>
        [Column("ReferenceCode")]
        public string ReferenceCurrencyCode { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MyCC.Core.Rates.ExchangeRate"/> reference currency is a crypto currency.
        /// </summary>
        /// <value><c>true</c> if reference currency is crypto currency; otherwise, <c>false</c>.</value>
        [Column("ReferenceIsCrypto")]
        public bool ReferenceCurrencyIsCryptoCurrency { get; }

        /// <summary>
        /// The secondary currency.
        /// </summary>
        [Column("SecondaryCode")]
        public string SecondaryCurrencyCode { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:MyCC.Core.Rates.ExchangeRate"/> secondary currency is a crypto currency.
        /// </summary>
        /// <value><c>true</c> if secondary currency is crypto currency; otherwise, <c>false</c>.</value>
        [Column("SecondaryIsCrypto")]
        public bool SecondaryCurrencyIsCryptoCurrency { get; }

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
                var exchangeRate = new ExchangeRate(SecondaryCurrencyCode, SecondaryCurrencyIsCryptoCurrency, ReferenceCurrencyCode, ReferenceCurrencyIsCryptoCurrency) { RepositoryId = RepositoryId };
                if (Rate != null && Rate != 0)
                {
                    exchangeRate.Rate = 1 / Rate;
                }
                return exchangeRate;
            }
        }

        /// <summary>
        /// Returns if the exchange rate contains the specified currency code.
        /// </summary>
        /// <param name="currency">The specified currency code.</param>
        public bool Contains(Currency.Model.Currency currency)
        {
            return Contains(currency.Code, currency.IsCryptoCurrency);
        }

        /// <summary>
        /// Returns if the exchange rate contains the specified currency.
        /// </summary>
        /// <param name="currencyCode">The specified currency.</param>
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