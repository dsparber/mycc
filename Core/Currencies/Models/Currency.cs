using System;
using Newtonsoft.Json;

namespace MyCC.Core.Currencies.Models
{
    public class Currency
    {
        /// <summary>
        /// The abbriviation code of the currency (e.g. USD, BTC, EUR).
        /// </summary>
        public readonly string Code;

        /// <summary>
        /// Wether this instance is a crypto currency or not. Needed, since the code alone is not unique.
        /// </summary>
        public readonly bool IsCrypto;

        /// <summary>
        /// Generated property from Code and IsCrypto. Unique.
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Phonetic Name of the currency.
        /// </summary>
        public string Name;

        /// <summary>
        /// Flags, which identify for which balance source this currency can be used.
        /// </summary>
        public int BalanceSourceFlags;


        /// <summary>
        /// Preferred constructor for creating Currency objects.
        /// </summary>
        /// <param name="code">The abbriviation code of the currency (e.g. USD, BTC, EUR)</param>
        /// <param name="name">Phonetic Name of the currency</param>
        /// <param name="isCrypto">Wether this instance is a crypto currency or not</param>
        [JsonConstructor]
        public Currency(string code, string name, bool isCrypto)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException();

            Name = name;
            Code = code.ToUpper();
            IsCrypto = isCrypto;
            Id = $"{Code}{(IsCrypto ? 1 : 0)}";
        }

        /// <summary>
        /// Should only be used for quick lookups. Name is not supported.
        /// </summary>
        /// <param name="code">The abbriviation code of the currency (e.g. USD, BTC, EUR)</param>
        /// <param name="isCryptoCurrency">Wether this instance is a crypto currency or not</param>
        public Currency(string code, bool isCryptoCurrency) : this(code, null, isCryptoCurrency) { }

        /// <summary>
        /// Get a database object for this instance
        /// </summary>
        [JsonIgnore]
        public CurrencyDbm DbObject => new CurrencyDbm { Id = Id, BalanceSourceFlags = BalanceSourceFlags, Code = Code, CryptoCurrency = IsCrypto, Name = Name };

        [JsonIgnore]
        public bool IsFiat => !IsCrypto;


        public override bool Equals(object obj) => string.Equals(Id, (obj as Currency)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;
    }
}