﻿using System;
using System.Collections.Generic;
using MyCryptos;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyCryptos
{
	public class AccountsCollection
	{
		private static AccountsCollection instance{ get; set; }

		public static AccountsCollection Instance {
			get {
				if (instance == null) {
					instance = new AccountsCollection ();
				}
				return instance;
			}
		}


		public ObservableCollection<Account> Accounts{ get; }

		private AccountsCollection ()
		{
			Accounts = new ObservableCollection<Account> ();
		}

		public Money GetSum (Currency currency)
		{
			Money money = new Money{ Currency = currency, Amount = 0 };
			foreach (Account account in Accounts) {
				money += CurrencyConverter.convert (account.Money, currency);
			}
			return money;
		}

		public Money Sum {
			get {
				return GetSum (PermanentSettings.Instance.ReferenceCurrency);
			}
		}

		public async Task LoadAccounts ()
		{
			Accounts.Add (new Account{ Money = new Money { Amount = 30, Currency = Currency.BTC } });
			Accounts.Add (new Account{ Money = new Money { Amount = 1, Currency = Currency.EUR } });
			Accounts.Add (new Account{ Money = new Money { Amount = 3, Currency = Currency.USD } });
		}
	}
}

