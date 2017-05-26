using System;
using Newtonsoft.Json;
using SQLite;

// ReSharper disable once MemberCanBePrivate.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MyCC.Core.Currencies.Models
{
    [Table("Currencies")]
    public class CurrencyDbm
    {
        private string _id;

        [PrimaryKey, Column("Id")]
        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                if (_id != null) throw new InvalidOperationException("Id can not be changed after first assignment");

                _id = value;
            }
        }

        [Column("Code")]
        public string Code { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("IsCrypto")]
        public bool CryptoCurrency { get; set; }

        [Column("Flags")]
        public int BalanceSourceFlags { get; set; }

        [Ignore]
        [JsonIgnore]
        public Currency Currency => new Currency(Code, Name, CryptoCurrency) { BalanceSourceFlags = BalanceSourceFlags };

        public override bool Equals(object obj) => string.Equals(Id, (obj as Currency)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Code;
    }
}

