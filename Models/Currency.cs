using System;

namespace MyCryptos
{
	public class Currency
	{
		public string Name{ get; }

		public string Abbreviation { get; }

		public Currency (string name, string abbreviation)
		{
			this.Name = name;
			this.Abbreviation = abbreviation;
		}

		public override bool Equals (Object obj)
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Currency c = (Currency)obj;

			return c.Abbreviation.Equals (Abbreviation);
		}


		public override int GetHashCode ()
		{
			return Abbreviation.GetHashCode ();
		}


		public static readonly Currency EUR = new Currency ("Euro", "EUR");
		public static readonly Currency USD = new Currency ("US Dollar", "USD");
		public static readonly Currency BTC = new Currency ("Bitcoin", "BTC");

	}
}

