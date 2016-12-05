﻿using System;
using System.Threading.Tasks;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;

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

		public override Task LoadBalanceFromDatabase()
		{
			throw new NotImplementedException();
		}

		public override Task LoadTransactionsFromDatabase()
		{
			throw new NotImplementedException();
		}
	}
}
