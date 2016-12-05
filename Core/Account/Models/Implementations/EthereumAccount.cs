using System;
using System.Threading.Tasks;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;

namespace MyCryptos.Core.Account.Models.Implementations
{
	public class EthereumAccount : OnlineFunctionalAccount
	{
		private readonly EthereumAccountRepository repository;

		public EthereumAccount(int? id, string name, Money money, EthereumAccountRepository repository) : base(id, repository.Id, name, money)
		{
			this.repository = repository;
		}

		public override Task FetchBalanceOnline()
		{
			throw new NotImplementedException();
		}

		public override Task FetchTransactionsOnline()
		{
			throw new NotImplementedException();
		}

		public override Task LoadBalanceFromDatabase()
		{
			throw new NotImplementedException();
		}

		public override Task LoadTransactionsFromDatabase()
		{
			throw new NotImplementedException();
		}
	}
}
