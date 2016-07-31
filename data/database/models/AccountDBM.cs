using models;
using SQLite;

namespace data.database.models
{
	[Table("Accounts")]
	public class AccountDBM
	{
		public AccountDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal MoneyAmount { get; set; }

		public int CurrencyId { get; set; }

		public int RepositoryId { get; set; }

		public Account ToAccount(Currency currency)
		{
			return new Account(Id, Name, new Money(MoneyAmount, currency));
		}

		public AccountDBM(Account account, int repositoryId)
		{
			Id = account.Id.Value;
			Name = account.Name;
			MoneyAmount = account.Money.Amount;
			CurrencyId = account.Money.Currency.Id.Value;
			RepositoryId = repositoryId;
		}
			
	}
}