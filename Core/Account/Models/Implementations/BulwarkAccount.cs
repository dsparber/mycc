using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;

namespace MyCC.Core.Account.Models.Implementations
{
    public class BulwarkAccount : OnlineFunctionalAccount
    {
        private readonly BulwarkAccountRepository _repository;

        public BulwarkAccount(int? id, string name, Money money, bool isEnabled, DateTime lastUpdate, BulwarkAccountRepository repository) : base(id, repository.Id, name, money, lastUpdate, isEnabled)
        {
            _repository = repository;
        }

        protected override Task FetchBalanceOnlineTask()
        {
            return _repository.FetchOnline();
        }

        public override Task FetchTransactionsOnline()
        {
            throw new NotImplementedException();
        }
    }
}
