using System;

namespace MyCryptos.Core.Account.Models.Base
{
    /// <summary>
    /// Simple Model for Money, includes basic arithmetics
    /// </summary>
    /// 
    public class Money
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Money"/> class.
        /// </summary>
        /// <param name="amount">Units of the currency for initialisation</param>
        /// <param name="currency">Desired currency</param>
        public Money(decimal amount, Currency.Model.Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException();
            }
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Money"/> class. The amount will be 0
        /// </summary>
        /// <param name="currency">Desired currency</param>
        public Money(Currency.Model.Currency currency) : this(0, currency) { }

        /// <summary>
        /// Holds the currency of the account
        /// </summary>
        /// <value>The currency to be set</value>
        public Currency.Model.Currency Currency { get; private set; }

        /// <summary>
        /// The current amount
        /// </summary>
        public readonly decimal Amount;

        /// <summary>
        /// Formats the money as string
        /// </summary>
        /// <returns>Money object as string</returns>
        public override string ToString()
        {
            return $"{Amount:#,0.########} {Currency.Code}";
        }

        public string ToStringTwoDigits()
        {
            return $"{(Amount < 0.01M && Amount > 0 ? $"> {0.01}" : $"{Amount:#,0.00}")} {Currency.Code}";
        }

        /// <summary>
        /// Adds a <see cref="Money"/> to a <see cref="Money"/>, yielding a new <see cref="T:Money"/>.
        /// </summary>
        /// <param name="m1">The first <see cref="Money"/> to add.</param>
        /// <param name="m2">The second <see cref="Money"/> to add.</param>
        /// <returns>The <see cref="T:Money"/> that is the sum of the values of <c>m1</c> and <c>m2</c>.</returns>
        public static Money operator +(Money m1, Money m2)
        {
            CheckForCurrencyMissmatch(m1, m2);
            return new Money((m1.Amount + m2.Amount), m1.Currency);
        }

        /// <summary>
        /// Subtracts a <see cref="Money"/> from a <see cref="Money"/>, yielding a new <see cref="T:Money"/>.
        /// </summary>
        /// <param name="m1">The <see cref="Money"/> to subtract from (the minuend).</param>
        /// <param name="m2">The <see cref="Money"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="T:Money"/> that is the <c>m1</c> minus <c>m2</c>.</returns>
        public static Money operator -(Money m1, Money m2)
        {
            CheckForCurrencyMissmatch(m1, m2);
            return new Money((m1.Amount - m2.Amount), m1.Currency);
        }

        /// <summary>
        /// Computes the product of <c>m1</c> and <c>m2</c>, yielding a new <see cref="T:Money"/>.
        /// </summary>
        /// <param name="m1">The <see cref="Money"/> to multiply.</param>
        /// <param name="m2">The <see cref="Money"/> to multiply.</param>
        /// <returns>The <see cref="T:Money"/> that is the <c>m1</c> * <c>m2</c>.</returns>
        public static Money operator *(Money m1, Money m2)
        {
            CheckForCurrencyMissmatch(m1, m2);
            return new Money((m1.Amount * m2.Amount), m1.Currency);
        }

        /// <summary>
        /// Computes the division of <c>m1</c> and <c>m2</c>, yielding a new <see cref="T:Money"/>.
        /// </summary>
        /// <param name="m1">The <see cref="Money"/> to divide (the divident).</param>
        /// <param name="m2">The <see cref="Money"/> to divide (the divisor).</param>
        /// <returns>The <see cref="T:Money"/> that is the <c>m1</c> / <c>m2</c>.</returns>
        public static Money operator /(Money m1, Money m2)
        {
            CheckForCurrencyMissmatch(m1, m2);
            return new Money((m1.Amount / m2.Amount), m1.Currency);
        }

        /// <summary>
        /// Checks for currency missmatch.
        /// </summary>
        /// <param name="m1">The first money instance</param>
        /// <param name="m2">The second money instance</param>
        private static void CheckForCurrencyMissmatch(Money m1, Money m2)
        {
            if (!m1.Currency.Equals(m2.Currency))
            {
                throw new InvalidOperationException("Currency missmatch - This operation cannot be performed on money instances with different currencies");
            }
        }
    }
}