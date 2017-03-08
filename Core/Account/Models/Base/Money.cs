using System;

namespace MyCC.Core.Account.Models.Base
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
            Amount = (long)Math.Truncate(amount * 10e8m) / 10e8m;
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
            return ToString(true);
        }

        public string ToString(bool showCurrency)
        {
            return $"{Amount:#,0.########}{(showCurrency ? $" {Currency.Code}" : string.Empty)}";
        }

        public string ToStringTwoDigits(bool round, bool showCurrency = true)
        {
            var amount = round ? Math.Round(Amount, 2) : Math.Truncate(Amount * 100) / 100;
            return $"{(round && Amount < 0.01M && Amount > 0 ? $"< {0.01}" : $"{amount:#,0.00}")}{ (showCurrency ? $" {Currency.Code}" : string.Empty)}";
        }

        public string ToString8Digits(bool showCurrency = true)
        {
            var amount = Math.Truncate(Amount * 100000000) / 100000000;
            return $"{amount:#,0.00000000}{(showCurrency ? $" {Currency.Code}" : string.Empty)}";
        }
    }
}