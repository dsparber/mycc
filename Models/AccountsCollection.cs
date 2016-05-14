using System;
using System.Collections.Generic;
using MyCryptos;

namespace MyCryptos
{
	public class AccountsCollection
	{
		List<Account> Accounts{ get; }

		public AccountsCollection ()
		{
			Accounts = new List<Account> ();
		}

		public Money getMoney (Currency currency)
		{
			Money money = new Money{ Currency = currency, Amount = 0 };
			foreach (Account account in Accounts) {
				money += CurrencyConverter.convert (account.Money, currency);
			}
			return money;
		}
	}
}

