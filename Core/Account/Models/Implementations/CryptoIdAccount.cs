using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;

namespace MyCC.Core.Account.Models.Implementations
{
    public class CryptoIdAccount : OnlineFunctionalAccount
    {
        private readonly CryptoIdAccountRepository _repository;

        public CryptoIdAccount(int? id, string name, Money money, CryptoIdAccountRepository repository) : base(id, repository.Id, name, money)
        {
            this._repository = repository;
        }

        public override Task FetchBalanceOnline()
        {
            return _repository.FetchOnline();
        }

        public override Task FetchTransactionsOnline()
        {
            throw new NotImplementedException();
        }
    }
}
