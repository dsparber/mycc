using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using SQLite;

namespace MyCC.Core.Account.Database
{
    [Table("AccountRepositories")]
    public class AccountRepositoryDbm : IEntityDbm<AccountRepository, int>
    {
        public const int DbTypeLocalRepository = 1;
        public const int DbTypeBittrexRepository = 2;
        public const int DbTypeBlockExpertsRepository = 3;
        public const int DbTypeBlockchainRepository = 4;
        public const int DbTypeEthereumRepository = 5;
        public const int DbTypeCryptoidRepository = 6;
        public const int DbTypeBlockchainXpubRepository = 7;
        public const int DbTypeEthereumClassicRepository = 8;

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Type")]
        public int Type { get; set; }

        /// <summary>
        /// Json formated additional data for the account. E.g. Api-Key
        /// </summary>
        /// <value>Json data</value>
        public string Data { get; set; }

        public AccountRepositoryDbm() { }

        public AccountRepositoryDbm(AccountRepository repository)
        {
            Name = repository.Name;
            Data = repository.Data;
            Type = repository.RepositoryTypeId;
            Id = repository.Id;
        }

        public override bool Equals(object obj)
        {
            var dbm = obj as AccountRepositoryDbm;
            return Id == dbm?.Id;
        }

        public override int GetHashCode() => 1;

        public Task<AccountRepository> Resolve()
        {
            return Task.Factory.StartNew<AccountRepository>(() =>
            {
                switch (Type)
                {
                    case DbTypeLocalRepository: return new LocalAccountRepository(Id, Name);
                    case DbTypeBittrexRepository: return new BittrexAccountRepository(Id, Name, Data);
                    case DbTypeBlockExpertsRepository: return new BlockExpertsAccountRepository(Id, Name, Data);
                    case DbTypeBlockchainRepository: return new BlockchainAccountRepository(Id, Name, Data);
                    case DbTypeEthereumRepository: return new EthereumAccountRepository(Id, Name, Data);
                    case DbTypeCryptoidRepository: return new CryptoIdAccountRepository(Id, Name, Data);
                    case DbTypeBlockchainXpubRepository: return new BlockchainXpubAccountRepository(Id, Name, Data);
                    case DbTypeEthereumClassicRepository: return new EthereumClassicAccountRepository(Id, Name, Data);
                    default: throw new NotSupportedException();
                }
            });
        }
    }
}