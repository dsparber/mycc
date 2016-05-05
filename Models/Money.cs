using System;

namespace MyCryptos
{
	public class Money
	{
		public decimal Amount{ get; set; }

		public Currency Currency { get; set; }


		// Operators

		public static Money operator + (Money m1, Money m2)
		{
			if (m1.Currency != m2.Currency)
				throw new Exception ("Currency missmatch");
			
			return new Money { Currency = m1.Currency, Amount = m1.Amount + m2.Amount };
		}

		public static Money operator - (Money m1, Money m2)
		{
			if (m1.Currency != m2.Currency)
				throw new Exception ("Currency missmatch");

			return new Money { Currency = m1.Currency, Amount = m1.Amount - m2.Amount };
		}

		public static Money operator * (Money m1, Money m2)
		{
			if (m1.Currency != m2.Currency)
				throw new Exception ("Currency missmatch");

			return new Money { Currency = m1.Currency, Amount = m1.Amount * m2.Amount };
		}

		public static Money operator / (Money m1, Money m2)
		{
			if (m1.Currency != m2.Currency)
				throw new Exception ("Currency missmatch");

			return new Money { Currency = m1.Currency, Amount = m1.Amount / m2.Amount };
		}
	}
}

