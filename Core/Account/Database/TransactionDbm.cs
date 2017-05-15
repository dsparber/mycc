using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using SQLite;

namespace MyCC.Core.Account.Database
{
    [Table("Transactions")]
    public class TransactionDbm : IEntityRepositoryIdDbm<Transaction, string>
    {
        public TransactionDbm() { }

        [PrimaryKey, Column("_id")]
        public string Id { get; set; }

        [Column("Timestamp")]
        private DateTime Timestamp { get; set; }

        [Column("Amount")]
        private decimal MoneyAmount { get; set; }

        [Column("Code")]
        private string CurrencyId { get; set; }

        public int ParentId { get; set; }

        public Task<Transaction> Resolve()
        {
            return new Task<Transaction>(() =>
            {
                var currency = CurrencyStorage.Find(CurrencyId);
                return new Transaction(Id, Timestamp, new Money(MoneyAmount, currency), ParentId);
            });
        }

        public TransactionDbm(Transaction transaction)
        {
            Id = transaction.Id;
            ParentId = transaction.ParentId;

            Timestamp = transaction.Timestamp;
            MoneyAmount = transaction.Money.Amount;
            CurrencyId = transaction.Money.Currency.Id;
        }
    }
}