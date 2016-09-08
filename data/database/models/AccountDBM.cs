using System.Threading.Tasks;
using data.database.interfaces;
using models;
using SQLite;

namespace data.database.models
{
	[Table("Accounts")]
	public class AccountDBM : IEntityRepositoryIdDBM<Account>
	{
		public AccountDBM() { }

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal MoneyAmount { get; set; }

		public int CurrencyId { get; set; }

		public int RepositoryId { get; set; }

		public async Task<Account> Resolve()
		{
			var db = new CurrencyDatabase();
			return new Account(Id, Name, new Money(MoneyAmount, await db.Get(CurrencyId)));  
		}

		public AccountDBM(Account account, int repositoryId)
		{
			if (account.Id.HasValue)
			{
				Id = account.Id.Value;
			}
			if (account.Money.Currency != null && account.Money.Currency.Id.HasValue)
			{
				CurrencyId = account.Money.Currency.Id.Value;
			}
			Name = account.Name;
			MoneyAmount = account.Money.Amount;
			RepositoryId = repositoryId;
		}
			
	}
}