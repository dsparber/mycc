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

        public CurrencyDbm(Model.Currency currency)
        {
            Name = currency.Name;
            Id = currency.Code;
        }

        public Task<Model.Currency> Resolve()
        {
            return Task.Factory.StartNew(() => new Model.Currency(Id, Name));
        }
    }
}

