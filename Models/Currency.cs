using System;

namespace Models
{
	/// <summary>
	///  Simple model for Currencies
	/// </summary>
	public class Currency
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Currency"/> class.
		/// </summary>
		/// <param name="abbreviation">Unique identifier if the currency</param>
		/// <param name="name">Name of the currency</param>
		public Currency(String abbreviation, String name)
		{
			Name = name;
			Abbreviation = abbreviation;
		}

		/// <summary>
		/// The name of the Currency
		/// </summary>
		public String Name;

		/// <summary>
		/// Stores the unique code for the currency
		/// </summary>
		/// <value>The abbreviation.</value>
		public String Abbreviation { get; private set; }
	}
}