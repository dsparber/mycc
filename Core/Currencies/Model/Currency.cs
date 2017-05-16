using System;
using Newtonsoft.Json;

namespace MyCC.Core.Currencies.Model
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
        public readonly bool CryptoCurrency;

        /// <summary>
        /// Generated property from Code and CryptoCurrency. Unique.
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
        /// <param name="cryptoCurrency">Wether this instance is a crypto currency or not</param>
        [JsonConstructor]
        public Currency(string code, string name, bool cryptoCurrency)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException();

            Name = name;
            Code = code.ToUpper();
            CryptoCurrency = cryptoCurrency;
            Id = $"{Code}{(CryptoCurrency ? 1 : 0)}";
        }

        /// <summary>
        /// Should only be used for quick lookups. Name is not supported.
        /// </summary>
        /// <param name="code">The abbriviation code of the currency (e.g. USD, BTC, EUR)</param>
        /// <param name="isCryptoCurrency">Wether this instance is a crypto currency or not</param>
        public Currency(string code, bool isCryptoCurrency) : this(code, null, isCryptoCurrency) { }

        /// <summary>
        /// Should only be used for quick lookups. Name is not supported.
        /// </summary>
        /// <param name="id">The id of the currency (e.g. USD0, BTC1, EUR0)</param>
        public Currency(string id) : this(id.Substring(0, id.Length - 1), null, id[id.Length - 1] == '1') { }


        /// <summary>
        /// Get a database object for this instance
        /// </summary>
        [JsonIgnore]
        public CurrencyDbm DbObject => new CurrencyDbm { Id = Id, BalanceSourceFlags = BalanceSourceFlags, Code = Code, CryptoCurrency = CryptoCurrency, Name = Name };



        public override bool Equals(object obj) => string.Equals(Id, (obj as Currency)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Code;
    }
}