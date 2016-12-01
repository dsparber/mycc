using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database.Models
{
    [Table("Accounts")]
    public class AccountDBM : IEntityRepositoryIdDBM<Account, int>
    {
        public AccountDBM() { }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal MoneyAmount { get; set; }

        public string CurrencyCode { get; set; }

        public int RepositoryId { get; set; }

        public async Task<Account> Resolve()
        {
            var db = new CurrencyDatabase();
            return new Account(Id, Name, new Money(MoneyAmount, (await db.Get(CurrencyCode)))) { RepositoryId = RepositoryId };
        }

        public AccountDBM(Account account)
        {

            Id = account.Id;

            if (account.Money.Currency != null)
            {
                CurrencyCode = account.Money.Currency.Code;
            }
            Name = account.Name;
            MoneyAmount = account.Money.Amount;
            RepositoryId = account.RepositoryId;
        }
    }
}