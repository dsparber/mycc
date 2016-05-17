using System;
using System.Collections.Generic;

namespace MyCryptos
{
	public class Account
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public List<Tag> Tags { get; set; }

		public Money Money { get; set; }

		public Money ReferenceValue { 
			get { 
				return CurrencyConverter.convert (Money, PermanentSettings.Instance.ReferenceCurrency);
			}
		}
	}
}

