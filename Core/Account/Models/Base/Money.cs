using System;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;

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
        public Money(decimal amount, Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException();
            }
            Currency = currency;

            try
            {

                Amount = Math.Truncate(amount * 10e7m) / 10e7m;
            }
            catch
            {
                Amount = amount;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Money"/> class. The amount will be 0
        /// </summary>
        /// <param name="currency">Desired currency</param>
        public Money(Currency currency) : this(0, currency) { }

        /// <summary>
        /// Holds the currency of the account
        /// </summary>
        /// <value>The currency to be set</value>
        public Currency Currency { get; private set; }

        /// <summary>
        /// The current amount
        /// </summary>
        public readonly decimal Amount;

        /// <summary>
        /// Formats the money as string
        /// </summary>
        /// <returns>Money object as string</returns>
        public override string ToString() => ToString(true);

        public string ToString(bool showCurrency)
        {
            return $"{Amount.ToMax8DigitString()}{(showCurrency ? $" {Currency.Code}" : string.Empty)}";
        }

        public string MaxTwoDigits(bool showCurrency = true)
        {
            var amount = Math.Truncate(Amount * 100) / 100;
            var noDigits = Amount == Math.Truncate(Amount);
            return $"{(noDigits ? Amount.To0DigitString() : amount.To2DigitString())}{Suffix(showCurrency)}";
        }

        public string TwoDigits(bool showCurrency = true)
        {
            var amount = Math.Truncate(Amount * 100) / 100;
            return $"{amount.To2DigitString()}{Suffix(showCurrency)}";
        }

        public string EightDigits(bool showCurrency = true)
        {
            var amount = Math.Truncate(Amount * 100000000) / 100000000;
            return $"{amount.To8DigitString()}{Suffix(showCurrency)}";
        }

        private string Suffix(bool showCurrency) => showCurrency ? $"\u00A0{Currency.Code}" : string.Empty;
    }
}