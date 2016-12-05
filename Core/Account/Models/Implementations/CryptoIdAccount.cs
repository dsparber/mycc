using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;

namespace MyCryptos.Core.Account.Models.Implementations
{
    public class CryptoIdAccount : OnlineFunctionalAccount
    {
        private readonly CryptoIdAccountRepository repository;

        public CryptoIdAccount(int? id, string name, Money money, CryptoIdAccountRepository repository) : base(id, repository.Id, name, money)
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
    }
}
