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
    public class AccountDbm : IEntityRepositoryIdDBM<FunctionalAccount, int>
    {
        public AccountDbm() { }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Amount")]
        public decimal MoneyAmount { get; set; }

        [Column("Code")]
        public string CurrencyCode { get; set; }

        [Column("AccountRepository")]
        public int ParentId { get; set; }

        public async Task<FunctionalAccount> Resolve()
        {
            var currency = CurrencyStorage.Instance.AllElements.Find(c => c?.Code.Equals(CurrencyCode) ?? false);
            if (currency == null)
            {
                var db = new CurrencyDatabase();
                currency = await db.Get(CurrencyCode);
            }

            var repository = AccountStorage.Instance.Repositories.Find(r => r.Id == ParentId);
            if (repository == null)
            {
                var db = new AccountRepositoryDatabase();
                repository = await db.Get(ParentId);
            }

            var money = new Money(MoneyAmount, currency);

            if (repository is BittrexAccountRepository) return new BittrexAccount(Id, Name, money, (BittrexAccountRepository)repository);
            if (repository is BlockchainAccountRepository) return new BlockchainAccount(Id, Name, money, (BlockchainAccountRepository)repository);
            if (repository is BlockExpertsAccountRepository) return new BlockExpertsAccount(Id, Name, money, (BlockExpertsAccountRepository)repository);
            if (repository is CryptoIdAccountRepository) return new CryptoIdAccount(Id, Name, money, (CryptoIdAccountRepository)repository);
            if (repository is EthereumAccountRepository) return new EthereumAccount(Id, Name, money, (EthereumAccountRepository)repository);
            if (repository is LocalAccountRepository) return new LocalAccount(Id, Name, money, repository.Id);
            throw new System.NotSupportedException();
        }

        public AccountDbm(FunctionalAccount account)
        {

            Id = account.Id;

            if (account.Money.Currency != null)
            {
                CurrencyCode = account.Money.Currency.Code;
            }
            Name = account.Name;
            MoneyAmount = account.Money.Amount;
            ParentId = account.ParentId;
        }
    }
}