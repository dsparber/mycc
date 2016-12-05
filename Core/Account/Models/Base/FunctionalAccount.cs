using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Account.Models
{
	public abstract class FunctionalAccount : Core.Models.Account
	{
		protected readonly AccountDatabase accountDatabase;
		protected readonly TransactionDatabase transactionDatabase;

		public List<Transaction> Transactions { get; private set; }

		protected FunctionalAccount(int? id, int repositoryId, string name, Money money) : base(id ?? default(int), repositoryId, name, money)
		{
			Transactions = new List<Transaction>();

			accountDatabase = new AccountDatabase();
			transactionDatabase = new TransactionDatabase();
		}

		public async Task LoadBalanceFromDatabase()
		{
			Money = (await accountDatabase.Get(Id)).Money;
		}
		public async Task LoadTransactionsFromDatabase()
		{
			Transactions = (await transactionDatabase.Get((TransactionDbm t) => t.ParentId == Id)).ToList();
		}
	}
}
