﻿using System;
using System.Threading.Tasks;

namespace data.repositories.account
{
	public class BittrexAccountRepository : OnlineAccountRepository
	{
		public override async Task Fetch()
		{
			// TODO Implement
			throw new NotImplementedException();

			await WriteToDatabase();
		}
	}
}

