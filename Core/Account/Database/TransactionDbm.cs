using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Storage;
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
        private string CurrencyCode { get; set; }

        public int ParentId { get; set; }

        public async Task<Transaction> Resolve()
        {
            var currency = CurrencyStorage.Instance.AllElements.Find(c => c.Code.Equals(CurrencyCode));
            if (currency != null) return new Transaction(Id, Timestamp, new Money(MoneyAmount, currency), ParentId);

            var db = new CurrencyDatabase();
            currency = await db.Get(CurrencyCode);

            return new Transaction(Id, Timestamp, new Money(MoneyAmount, currency), ParentId);
        }

        public TransactionDbm(Transaction transaction)
        {
            Id = transaction.Id;
            ParentId = transaction.ParentId;

            Timestamp = transaction.Timestamp;
            MoneyAmount = transaction.Money.Amount;
            CurrencyCode = transaction.Money.Currency.Code;
        }
    }
}