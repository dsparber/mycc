using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Currency.Repositories;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    [Table("CurrencyRepositoryMap")]
    public class CurrencyMapRepositoryDbm : IEntityDBM<CurrencyRepositoryMap, int>
    {

        public CurrencyMapRepositoryDbm() { }

        public CurrencyMapRepositoryDbm(CurrencyRepositoryMap repository)
        {
            Id = repository.Id;
        }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public Task<CurrencyRepositoryMap> Resolve()
        {
            return Task.Factory.StartNew(() =>
            {
                return new CurrencyRepositoryMap { Id = Id };
            });
        }
    }
}

