using System;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    [Table("Currencies")]
    public class CurrencyDBM : IEntityRepositoryIdDBM<Model.Currency, string>
    {
        public CurrencyDBM() { }

        [Ignore]
        public int ParentId
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public string Name { get; set; }

        [PrimaryKey, Column("_id")]
        public string Id { get; set; }

        public CurrencyDBM(Model.Currency currency)
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

