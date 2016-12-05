using System;
using MyCryptos.Core.Models;

namespace MyCryptos.Core.Account.Models
{
	public class Transaction
	{
		public readonly DateTime Timestamp;
		public readonly Money Money;

		public Transaction(DateTime dateTime, Money money)
		{
			Timestamp = dateTime;
			Money = money;
		}
	}
}
