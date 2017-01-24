using System;
using System.Globalization;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Implementations;

namespace MyCC.Core.Account.Models.Implementations
{
    public sealed class BittrexAccount : OnlineFunctionalAccount
    {
        private readonly BittrexAccountRepository _repository;
        private const string BalanceKey = "Balance";


        public BittrexAccount(int? id, string name, Money money, BittrexAccountRepository repository) : base(id, repository.Id, name, money)
        {
            this._repository = repository;
        }

        public override async Task FetchBalanceOnline()
        {
            var result = (await _repository.GetResult(Money.Currency));
            var balance = decimal.Parse((string)result[BalanceKey], CultureInfo.InvariantCulture);
            Money = new Money(balance, Money.Currency);
            await AccountDatabase.Update(this);
        }

        public override Task FetchTransactionsOnline()
        {
            throw new NotImplementedException();
        }
    }
}
