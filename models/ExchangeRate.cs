namespace MyCryptos.models
{

	/// <summary>
	/// Simple exchange rate model
	/// </summary>
	public class ExchangeRate : PersistableRepositoryElement
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Models.ExchangeRate"/> class.
		/// </summary>
		/// <param name="referenceCurrency">Reference currency.</param>
		/// <param name="secondaryCurrency">Secondary currency.</param>
		/// <param name="rate">Exchange rate.</param>
		public ExchangeRate(Currency referenceCurrency, Currency secondaryCurrency, decimal? rate)
		{
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
		public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the repository identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int? RepositoryId { get; set; }

		/// <summary>
		/// The reference currency.
		/// </summary>
		public Currency ReferenceCurrency;

		/// <summary>
		/// The secondary currency.
		/// </summary>
		public Currency SecondaryCurrency;

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
		public ExchangeRate GetInverse()
		{
			var exchangeRate = new ExchangeRate(SecondaryCurrency, ReferenceCurrency);
			if (Rate != null)
			{
				exchangeRate.Rate = 1 / Rate;
			}
			return exchangeRate;
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

			return ((ReferenceCurrency == null && r.ReferenceCurrency == null) || (ReferenceCurrency != null && ReferenceCurrency.Equals(r.ReferenceCurrency)))
				&& ((SecondaryCurrency == null && r.SecondaryCurrency == null) || (SecondaryCurrency != null && SecondaryCurrency.Equals(r.SecondaryCurrency)));
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			if (ReferenceCurrency == null && SecondaryCurrency == null) { return 0; }
			if (ReferenceCurrency != null) { return ReferenceCurrency.GetHashCode(); }
			if (SecondaryCurrency != null) { return SecondaryCurrency.GetHashCode(); }

			return ReferenceCurrency.GetHashCode() + SecondaryCurrency.GetHashCode();
		}

		/// <summary>
		/// Get the exchange rate as String.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[ExchangeRate: ReferenceCurrency={0}, SecondaryCurrency={1}, Rate={2}]", ReferenceCurrency, SecondaryCurrency, Rate);
		}
	}
}