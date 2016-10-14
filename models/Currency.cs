using Newtonsoft.Json;
namespace models
{
	/// <summary>
	///  Simple model for Currencies
	/// </summary>
	public class Currency
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Currency"/> class.
		/// </summary>
		/// <param name="id">Unique identifier if the object</param>
		/// <param name="code">Unique identifier if the currency</param>
		/// <param name="name">Name of the currency</param>
		[JsonConstructor]
		public Currency(int? id, string code, string name)
		{
			Id = id;
			Name = name;
			Code = code.ToUpper();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:models.Currency"/> class.
		/// </summary>
		/// <param name="code">Unique identifier if the currency</param>
		/// <param name="name">Name of the currency</param>
		public Currency(string code, string name) : this(null, code, name) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:models.Currency"/> class.
		/// </summary>
		/// <param name="code">Unique identifier if the currency</param>
		public Currency(string code) : this(code, code) { }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int? Id { get; set; }

		/// <summary>
		/// The name of the Currency
		/// </summary>
		public string Name;

		/// <summary>
		/// Stores the unique code for the currency
		/// </summary>
		/// <value>The abbreviation.</value>
		public string Code { get; private set; }

		/// <summary>
		/// Equals the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Currency))
			{
				return false;
			}
			var c = (Currency)obj;

			return Code.Equals(c.Code);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:models.Currency"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:models.Currency"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Currency: Id={0}, Code={1}]", Id, Code);
		}

		public static readonly Currency BTC = new Currency("BTC", "Bitcoin");
		public static readonly Currency EUR = new Currency("EUR", "Euro");
		public static readonly Currency USD = new Currency("USD", "US Dollar");
	}
}