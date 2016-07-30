using models;
using SQLite;

namespace data.database.models
{
	[Table("Accounts")]
	public class AccountDBM
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal MoneyAmount { get; set; }

		[MaxLength(3)]
		public string CurrencyCode { get; set; }

		public int RepositoryId { get; set; }

		public Account ToAccount()
		{
			return new Account(Name, new Money(MoneyAmount, new Currency(CurrencyCode)));
		}
			
	}
}