using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;

namespace MyCryptos.Core.Account.Models.Implementations
{
    public class BlockchainAccount : OnlineFunctionalAccount
    {
        private readonly BlockchainAccountRepository repository;

        public BlockchainAccount(int? id, string name, Money money, BlockchainAccountRepository repository) : base(id, repository.Id, name, money)
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
