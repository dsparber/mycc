using System;

namespace MyCryptos.models
{

	/// <summary>
	/// Simple exchange rate model
	/// </summary>
	public class ExchangeRate : PersistableRepositoryElement<string>
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class.
		/// </summary>
		/// <param name="referenceCurrency">Reference currency.</param>
		/// <param name="secondaryCurrency">Secondary currency.</param>
		/// <param name="rate">Exchange rate.</param>
		public ExchangeRate(Currency referenceCurrency, Currency secondaryCurrency, decimal? rate)
		{
			if (referenceCurrency == null || secondaryCurrency == null)
			{
				throw new ArgumentNullException();
			}
			ReferenceCurrency = referenceCurrency;
			SecondaryCurrency = secondaryCurrency;
			Rate = rate;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class without an exchange rate.
		/// </summary>
		/// <param name="referenceCurrency">Reference currency.</param>
		/// <param name="secondaryCurrency">Secondary currency.</param>
		public ExchangeRate(Currency referenceCurrency, Currency secondaryCurrency) : this(referenceCurrency, secondaryCurrency, null) { }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public string Id
		{
			get { return ReferenceCurrency.Code + SecondaryCurrency.Code; }
			set { }
		}

		/// <summary>
		/// Gets or sets the repository identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int RepositoryId { get; set; }

		/// <summary>
		/// The reference currency.
		/// </summary>
		public Currency ReferenceCurrency { get; private set; }

		/// <summary>
		/// The secondary currency.
		/// </summary>
		public Currency SecondaryCurrency { get; private set; }

		/// <summary>
		/// The rate, can not be not null. No value equals 0
		/// </summary>
		public decimal RateNotNull;

		/// <summary>
		/// Gets or sets the rate.
		/// </summary>
		/// <value>The exchange rate</value>
		public decimal? Rate
		{
			get
			{
				if (RateNotNull == 0)
				{
					return null;
				}
				return RateNotNull;
			}
			set
			{
				if (value != null && value > 0)
				{
					RateNotNull = (decimal)value;
				}
			}
		}


		/// <summary>
		/// Gets the inverse exchange rate.
		/// </summary>
		/// <returns>The inverse exchange rate.</returns>
		public ExchangeRate Inverse
		{
			get
			{
				var exchangeRate = new ExchangeRate(SecondaryCurrency, ReferenceCurrency);
				if (Rate != null)
				{
					exchangeRate.Rate = 1 / Rate;
				}
				return exchangeRate;
			}
		}

		/// <summary>
		/// Returns if the exchange rate contains the specified currency.
		/// </summary>
		/// <param name="currency">The specified currency.</param>
		public bool Contains(Currency currency)
		{
			return (ReferenceCurrency != null && ReferenceCurrency.Equals(currency)) || (SecondaryCurrency != null && SecondaryCurrency.Equals(currency));
		}

		/// <summary>
		/// Checks if the instance equals the specified object.
		/// </summary>
		/// <param name="obj">The comparison object.</param>
		public override bool Equals(object obj)
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;

			var r = (ExchangeRate)obj;

			if (ReferenceCurrency != null && SecondaryCurrency != null)
			{
				return ReferenceCurrency.Equals(r.ReferenceCurrency) && SecondaryCurrency.Equals(r.SecondaryCurrency);
			}
			return Id == r.Id;
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			if (ReferenceCurrency != null && SecondaryCurrency != null)
			{
				return ReferenceCurrency.GetHashCode() + SecondaryCurrency.GetHashCode();
			}
			return Id.GetHashCode();
		}

		/// <summary>
		/// Get the exchange rate as String.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("{0} -> {1}: {2}", ReferenceCurrency, SecondaryCurrency, Rate);
		}
	}
}