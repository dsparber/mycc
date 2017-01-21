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
    public class TransactionDbm : IEntityRepositoryIdDBM<Transaction, string>
    {
        public TransactionDbm() { }

        [PrimaryKey, Column("_id")]
        public string Id { get; set; }

        [Column("Timestamp")]
        private DateTime timestamp { get; set; }

        [Column("Amount")]
        private decimal moneyAmount { get; set; }

        [Column("Code")]
        private string currencyCode { get; set; }

        public int ParentId { get; set; }

        public async Task<Transaction> Resolve()
        {
            var currency = CurrencyStorage.Instance.AllElements.Find(c => c.Code.Equals(currencyCode));
            if (currency == null)
            {
                var db = new CurrencyDatabase();
                currency = await db.Get(currencyCode);
            }

            return new Transaction(Id, timestamp, new Money(moneyAmount, currency), ParentId);
        }

        public TransactionDbm(Transaction transaction)
        {
            Id = transaction.Id;
            ParentId = transaction.ParentId;

            timestamp = transaction.Timestamp;
            moneyAmount = transaction.Money.Amount;
            currencyCode = transaction.Money.Currency.Code;
        }
    }
}