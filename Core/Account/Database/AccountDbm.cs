using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using SQLite;

namespace MyCC.Core.Account.Database
{
    [Table("Accounts")]
    public class AccountDbm : IEntityRepositoryIdDbm<FunctionalAccount, int>
    {
        public AccountDbm() { }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        [Column("Name")]
        public string Name { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool? IsEnabled { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        [Column("Amount")]
        public decimal MoneyAmount { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        [Column("Code")]
        public string CurrencyId { get; set; }

        [Column("AccountRepository")]
        public int ParentId { get; set; }

        [Column("LastUpdate")]
        public long LastUpdateTicks { get; set; }


        public async Task<FunctionalAccount> Resolve()
        {
            var currency = CurrencyId.Find();

            var repository = AccountStorage.Instance.Repositories.Find(r => r.Id == ParentId);
            if (repository == null)
            {
                var db = new AccountRepositoryDatabase();
                repository = await db.Get(ParentId);
            }
            var lastUpdate = new DateTime(LastUpdateTicks);


            if (repository != null)
            {
                repository.LastFetch = lastUpdate;
            }

            var money = new Money(MoneyAmount, currency);


            if (repository is BittrexAccountRepository) return new BittrexAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BittrexAccountRepository)repository);
            if (repository is BlockchainAccountRepository) return new BlockchainAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockchainAccountRepository)repository);
            if (repository is BlockExpertsAccountRepository) return new BlockExpertsAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockExpertsAccountRepository)repository);
            if (repository is CryptoIdAccountRepository) return new CryptoIdAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (CryptoIdAccountRepository)repository);
            if (repository is EthereumAccountRepository) return new EthereumAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (EthereumAccountRepository)repository);
            if (repository is LocalAccountRepository) return new LocalAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, repository.Id);
            if (repository is BlockchainXpubAccountRepository) return new BlockchainXpubAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockchainXpubAccountRepository)repository);
            return null;
        }

        public AccountDbm(FunctionalAccount account)
        {

            Id = account.Id;

            if (account.Money.Currency != null)
            {
                CurrencyId = account.Money.Currency.Id;
            }
            Name = account.Name;
            MoneyAmount = account.Money.Amount;
            ParentId = account.ParentId;
            IsEnabled = account.IsEnabled;
            LastUpdateTicks = account.LastUpdate.Ticks;
        }
    }
}