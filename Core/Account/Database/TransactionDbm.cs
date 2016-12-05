using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Interfaces;
using MyCryptos.Core.Account.Models;
using SQLite;
using MyCryptos.Core.Storage;

namespace MyCryptos.Core.Database.Models
{
	[Table("Transactions")]
	public class TransactionDbm : IEntityRepositoryIdDBM<Transaction, string>
	{
		public TransactionDbm() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
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

			return new Transaction(Id, timestamp, new Core.Models.Money(moneyAmount, currency), ParentId);
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