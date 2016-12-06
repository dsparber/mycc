using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using System.Globalization;

namespace MyCryptos.Core.Account.Models.Implementations
{
	public sealed class BittrexAccount : OnlineFunctionalAccount
	{
		private readonly BittrexAccountRepository repository;
		private const string BalanceKey = "Balance";


		public BittrexAccount(int? id, string name, Money money, BittrexAccountRepository repository) : base(id, repository.Id, name, money)
		{
			this.repository = repository;
		}

		public override async Task FetchBalanceOnline()
		{
			var result = (await repository.GetResult(Money.Currency));
			var balance = decimal.Parse((string)result[BalanceKey], CultureInfo.InvariantCulture);
			Money = new Money(balance, Money.Currency);
			await accountDatabase.Update(this);
		}

		public override Task FetchTransactionsOnline()
		{
			throw new NotImplementedException();
		}
	}
}
