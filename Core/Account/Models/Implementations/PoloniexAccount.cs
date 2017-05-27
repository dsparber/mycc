using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;

namespace MyCC.Core.Account.Models.Implementations
{
    public sealed class PoloniexAccount : OnlineFunctionalAccount
    {
        private readonly PoloniexAccountRepository _repository;


        public PoloniexAccount(int? id, string name, Money money, bool isEnabled, DateTime lastUpdate, PoloniexAccountRepository repository) : base(id, repository.Id, name, money, lastUpdate, isEnabled)
        {
            _repository = repository;
        }

        protected override async Task FetchBalanceOnlineTask()
        {
            await _repository.FetchOnline();
        }

        public override Task FetchTransactionsOnline()
        {
            throw new NotImplementedException();
        }
    }
}
