using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;

namespace MyCryptos.Core.Account.Models.Implementations
{
	public class BlockchainXpubAccount : OnlineFunctionalAccount
	{
		private readonly BlockchainXpubAccountRepository repository;

		public BlockchainXpubAccount(int? id, string name, Money money, BlockchainXpubAccountRepository repository) : base(id, repository.Id, name, money)
		{
			this.repository = repository;
		}

		public override Task FetchBalanceOnline()
		{
			return repository.FetchOnline();
		}

		public override Task FetchTransactionsOnline()
		{
			throw new NotImplementedException();
		}
	}
}
