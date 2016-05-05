using System;
using System.Collections.Generic;

namespace MyCryptos
{
	public abstract class Account
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public double Units { get; set; }

		public List<Tag> Tags { get; set; }

		public Money getMoney (Currency currency);
	}
}

