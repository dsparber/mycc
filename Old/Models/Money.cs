﻿using System;
using System.Globalization;

namespace MyCryptos
{
	public class Money
	{
		public Money() { }
		public Money(decimal amount, Currency currency)
		{
			this.Amount = amount;
			this.Currency = currency;
		}

		public decimal Amount{ get; set; }

		public Currency Currency { get; set; }

		public string String {
			get {
				return String.Format (Settings.CultureInfo, "{0:#,0.##} {1}", Amount, Currency.Abbreviation);
			}
		}

		// Operators

		public static Money operator + (Money m1, Money m2)
		{
			if (m1.Currency != m2.Currency)
				throw new Exception ("Currency missmatch");
			
			return new Money {Currency = m1.Currency, Amount = m1.Amount + m2.Amount };
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
