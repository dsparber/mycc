using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;

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
            return repository.FetchOnline();
        }

        public override Task FetchTransactionsOnline()
        {
            throw new NotImplementedException();
        }
    }
}
