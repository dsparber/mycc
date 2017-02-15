using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Storage;
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
            var currency = CurrencyStorage.Instance.AllElements.Find(c => c?.Id.Equals(CurrencyId) ?? false) ?? CurrencyStorage.Instance.AllElements.Find(c => c?.Code.Equals(CurrencyId) ?? false);
            if (currency == null)
            {
                var db = new CurrencyDatabase();
                currency = await db.Get(CurrencyId);
                if (currency == null)
                {
                    var code = CurrencyId.EndsWith("0") || CurrencyId.EndsWith("1") ? CurrencyId.Substring(0, CurrencyId.Length - 1) : CurrencyId;
                    if (code.Equals(CurrencyId))
                    {
                        currency = await db.Get(code + "1") ?? await db.Get(code + "0") ?? new Currency.Model.Currency(code, false);
                    }
                    else
                    {
                        currency = new Currency.Model.Currency(code, CurrencyId[code.Length] == '1');
                    }
                }
            }


            var repository = AccountStorage.Instance.Repositories.Find(r => r.Id == ParentId);
            if (repository == null)
            {
                var db = new AccountRepositoryDatabase();
                repository = await db.Get(ParentId);
            }

            var money = new Money(MoneyAmount, currency);

            var lastUpdate = new DateTime(LastUpdateTicks);

            if (repository is BittrexAccountRepository) return new BittrexAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BittrexAccountRepository)repository);
            if (repository is BlockchainAccountRepository) return new BlockchainAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockchainAccountRepository)repository);
            if (repository is BlockExpertsAccountRepository) return new BlockExpertsAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockExpertsAccountRepository)repository);
            if (repository is CryptoIdAccountRepository) return new CryptoIdAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (CryptoIdAccountRepository)repository);
            if (repository is EthereumAccountRepository) return new EthereumAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (EthereumAccountRepository)repository);
            if (repository is LocalAccountRepository) return new LocalAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, repository.Id);
            if (repository is BlockchainXpubAccountRepository) return new BlockchainXpubAccount(Id, Name, money, IsEnabled ?? true, lastUpdate, (BlockchainXpubAccountRepository)repository);
            throw new NotSupportedException();
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