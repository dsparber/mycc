using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;

namespace MyCryptos.Core.Account.Models.Implementations
{
    public class BittrexAccount : OnlineFunctionalAccount
    {
        private readonly BittrexAccountRepository repository;

        public BittrexAccount(int? id, string name, Money money, BittrexAccountRepository repository) : base(id, repository.Id, name, money)
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
