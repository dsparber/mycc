using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Currency.Repositories;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    [Table("CurrencyRepositories")]
    public class CurrencyRepositoryDbm : IEntityDbm<CurrencyRepository, int>
    {
        public const int DbTypeLocalRepository = 1;
        public const int DbTypeBtceRepository = 2;
        public const int DbTypeBittrexRepository = 3;
        public const int DbTypeCryptonatorRepository = 4;
        public const int DbTypeBlockExpertsRepository = 5;
        public const int DbTypeCryptoidRepository = 6;
        public const int DbTypeOpenExchangeRepository = 7;


        public CurrencyRepositoryDbm() { }

        public CurrencyRepositoryDbm(CurrencyRepository repository)
        {
            Type = repository.RepositoryTypeId;
            Id = repository.Id;
        }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("Type")]
        public int Type { get; set; }

        public Task<CurrencyRepository> Resolve()
        {
            return Task.Factory.StartNew<CurrencyRepository>(() =>
            {
                switch (Type)
                {
                    case DbTypeLocalRepository: return new LocalCurrencyRepository(Id);
                    case DbTypeBittrexRepository: return new BittrexCurrencyRepository(Id);
                    case DbTypeBtceRepository: return new BtceCurrencyRepository(Id);
                    case DbTypeCryptonatorRepository: return new CryptonatorCurrencyRepository(Id);
                    case DbTypeBlockExpertsRepository: return new BlockExpertsCurrencyRepository(Id);
                    case DbTypeCryptoidRepository: return new CryptoIdCurrencyRepository(Id);
                    case DbTypeOpenExchangeRepository: return new OpenexchangeCurrencyRepository(Id);
                    default: throw new NotSupportedException();
                }
            });
        }
    }
}

