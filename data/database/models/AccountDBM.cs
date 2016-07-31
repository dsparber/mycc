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

		[MaxLength(3)]
		public string CurrencyCode { get; set; }

		public int RepositoryId { get; set; }

		public Account ToAccount()
		{
			return new Account(Id, Name, new Money(MoneyAmount, new Currency(CurrencyCode)));
		}

		public AccountDBM(Account account, int repositoryId)
		{
			Id = account.Id.Value;
			Name = account.Name;
			MoneyAmount = account.Money.Amount;
			CurrencyCode = account.Money.Currency.Abbreviation;
			RepositoryId = repositoryId;
		}
			
	}
}