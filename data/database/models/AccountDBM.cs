using System.Threading.Tasks;
using data.database.interfaces;
using MyCryptos.models;
using SQLite;

namespace data.database.models
{
	[Table("Accounts")]
	public class AccountDBM : IEntityRepositoryIdDBM<Account, int>
	{
		public AccountDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal MoneyAmount { get; set; }

		[MaxLength(3)]
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