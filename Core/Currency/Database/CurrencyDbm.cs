using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    [Table("Currencies")]
    public class CurrencyDbm : IEntityRepositoryIdDbm<Model.Currency, string>
    {
        public CurrencyDbm() { }

        [Ignore]
        public int ParentId
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        [PrimaryKey, Column("Code")]
        public string Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("IsCrypto")]
        public bool IsCryptoCurrency { get; set; }

        public CurrencyDbm(Model.Currency currency)
        {
            Name = currency.Name;
            IsCryptoCurrency = currency.IsCryptoCurrency;
            Id = currency.Code + (IsCryptoCurrency ? "1" : "0");
        }

        public async Task<Model.Currency> Resolve()
        {
            var code = Id.Substring(0, Id.Length - 1);
            if (!string.IsNullOrWhiteSpace(code) && code.Length >= 2) return new Model.Currency(code, Name, IsCryptoCurrency);

            await (await new CurrencyDatabase().Connection).DeleteAsync(this);
            return null;
        }
    }
}

