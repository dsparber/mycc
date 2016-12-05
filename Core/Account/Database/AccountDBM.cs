using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Models.Implementations;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Database;
using MyCryptos.Core.Currency.Storage;
using SQLite;

namespace MyCryptos.Core.Account.Database
{
    [Table("Accounts")]
    public class AccountDBM : IEntityRepositoryIdDBM<FunctionalAccount, int>
    {
        public AccountDBM() { }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal MoneyAmount { get; set; }

        public string CurrencyCode { get; set; }

        public int ParentId { get; set; }

        public async Task<FunctionalAccount> Resolve()
        {
            var currency = CurrencyStorage.Instance.AllElements.Find(c => c.Code.Equals(CurrencyCode));
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

        public AccountDBM(FunctionalAccount account)
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