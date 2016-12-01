using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("Currencies")]
    public class CurrencyDBM : IEntityRepositoryIdDBM<Currency, string>
    {
        public CurrencyDBM() { }

        [Ignore]
        public int RepositoryId
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public string Name { get; set; }

        [PrimaryKey, Column("_id")]
        public string Id { get; set; }

        public CurrencyDBM(Currency currency)
        {
            Name = currency.Name;
            Id = currency.Code;
        }

        public Task<Currency> Resolve()
        {
            return Task.Factory.StartNew(() => new Currency(Id, Name));
        }
    }
}

