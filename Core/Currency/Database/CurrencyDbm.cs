using System;
using System.Text.RegularExpressions;
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
            Id = currency.Code + (IsCryptoCurrency ? 1 : 0);
            IsCryptoCurrency = currency.IsCryptoCurrency;
        }

        public Task<Model.Currency> Resolve()
        {
            return Task.Factory.StartNew(() => new Model.Currency(Regex.Replace(Id, "[0-9]", ""), Name, IsCryptoCurrency));
        }
    }
}

