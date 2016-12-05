using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Account.Models
{
	public abstract class FunctionalAccount : Core.Models.Account
	{
		public readonly List<Transaction> Transactions;

		protected FunctionalAccount(int? id, int repositoryId, string name, Money money) : base(id ?? default(int), repositoryId, name, money)
		{
			Transactions = new List<Transaction>();
		}

		public abstract Task LoadBalanceFromDatabase();
		public abstract Task LoadTransactionsFromDatabase();
	}
}
