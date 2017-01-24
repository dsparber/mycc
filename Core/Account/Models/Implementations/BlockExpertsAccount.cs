using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;

namespace MyCC.Core.Account.Models.Implementations
{
    public class BlockExpertsAccount : OnlineFunctionalAccount
    {
        private readonly BlockExpertsAccountRepository _repository;

        public BlockExpertsAccount(int? id, string name, Money money, BlockExpertsAccountRepository repository) : base(id, repository.Id, name, money)
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
