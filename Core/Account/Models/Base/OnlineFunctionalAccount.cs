using System;
using System.Threading.Tasks;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Account.Models
{
	public abstract class OnlineFunctionalAccount : FunctionalAccount
	{
		public DateTime LastUpdate;

		protected OnlineFunctionalAccount(int? id, int repositoryId, string name, Money money) : base(id ?? default(int), repositoryId, name, money)
		{
			LastUpdate = DateTime.MinValue;
		}

		public abstract Task FetchBalanceOnline();
		public abstract Task FetchTransactionsOnline();
	}
}
