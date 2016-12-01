using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Repositories.AvailableRates;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("AvailableRatesRepositories")]
    public class AvailableRatesRepositoryDBM : IEntityDBM<AvailableRatesRepository, int>
    {
        public const int DB_TYPE_LOCAL_REPOSITORY = 1;
        public const int DB_TYPE_BTCE_REPOSITORY = 2;
        public const int DB_TYPE_BITTREX_REPOSITORY = 3;
        public const int DB_TYPE_CRYPTONATOR_REPOSITORY = 4;

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public AvailableRatesRepositoryDBM() { }

        public AvailableRatesRepositoryDBM(AvailableRatesRepository repository)
        {
            Name = repository.Name;
            Type = repository.RepositoryTypeId;
            Id = repository.Id;
        }

        public Task<AvailableRatesRepository> Resolve()
        {
            return Task.Factory.StartNew<AvailableRatesRepository>(() =>
            {
                switch (Type)
                {
                    case DB_TYPE_LOCAL_REPOSITORY: return new LocalAvailableRatesRepository(Name) { Id = Id };
                    case DB_TYPE_BITTREX_REPOSITORY: return new BittrexAvailableRatesRepository(Name) { Id = Id };
                    case DB_TYPE_BTCE_REPOSITORY: return new BtceAvailableRatesRepository(Name) { Id = Id };
                    case DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorAvailableRatesRepository(Name) { Id = Id };
                    default: throw new NotSupportedException();
                }
            });
        }
    }
}

